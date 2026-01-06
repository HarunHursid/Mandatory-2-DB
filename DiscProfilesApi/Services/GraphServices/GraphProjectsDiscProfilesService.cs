using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;

namespace DiscProfilesApi.Services.GraphServices
{
    public class GraphProjectsDiscProfilesService
    {
        private readonly IDriver _driver;
        private readonly DiscProfilesContext _sqlContext;

        public GraphProjectsDiscProfilesService(IDriver driver, DiscProfilesContext sqlContext)
        {
            _driver = driver;
            _sqlContext = sqlContext;
        }

        public async Task<IEnumerable<ProjectDiscProfileDetailDto>> GetDiscProfilesForProjectAsync(int projectId)
        {
            var result = new List<ProjectDiscProfileDetailDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (p:Project {id: $projectId})-[:HAS_PROFILE]->(dp:DiscProfile)
                RETURN dp.id AS profileId, dp.name AS profileName, dp.color AS color
                ORDER BY dp.name
            ", new { projectId });

            await foreach (var record in cursor)
            {
                result.Add(new ProjectDiscProfileDetailDto
                {
                    ProfileId = record["profileId"].As<int>(),
                    ProfileName = record["profileName"].As<string>(),
                    Color = record["color"].As<string>()
                });
            }

            return result;
        }

        public async Task MirrorProjectsDiscProfileFromSqlAsync(int linkId)
        {
            var link = await _sqlContext.projects_disc_profiles
                .Include(p => p.project)
                .Include(p => p.disc_profile)
                .FirstOrDefaultAsync(p => p.id == linkId);

            if (link == null)
                return;

            await using var session = _driver.AsyncSession();

            const string linkCypher = @"
MATCH (p:Project {id: $projectId})
MATCH (dp:DiscProfile {id: $discProfileId})
MERGE (p)-[:HAS_PROFILE]->(dp);
";

            await session.RunAsync(linkCypher, new
            {
                projectId = link.project_id,
                discProfileId = link.disc_profile_id
            });
        }


        public async Task<bool> UpdateProjectDiscProfileRelationAsync(int projectId, int oldDiscProfileId, int newDiscProfileId)
        {
            await using var session = _driver.AsyncSession();

            // Delete old relationship and create new one
            var result = await session.RunAsync(@"
                MATCH (p:Project {id: $projectId})-[rel:HAS_PROFILE]->(dp:DiscProfile {id: $oldDiscProfileId})
                DELETE rel
                WITH p
                MATCH (newDp:DiscProfile {id: $newDiscProfileId})
                MERGE (p)-[:HAS_PROFILE]->(newDp)
            ", new { projectId, oldDiscProfileId, newDiscProfileId });

            var summary = await result.ConsumeAsync();
            return summary.Counters.RelationshipsDeleted > 0 && summary.Counters.RelationshipsCreated > 0;
        }

        // Deletes the relationship between Project and DiscProfile
        public async Task<bool> DeleteProjectDiscProfileRelationAsync(int projectId, int discProfileId)
        {
            await using var session = _driver.AsyncSession();

            var result = await session.RunAsync(@"
                MATCH (p:Project {id: $projectId})-[rel:HAS_PROFILE]->(dp:DiscProfile {id: $discProfileId})
                DELETE rel
            ", new { projectId, discProfileId });

            var summary = await result.ConsumeAsync();
            return summary.Counters.RelationshipsDeleted > 0;
        }
    }

    public class ProjectDiscProfileDetailDto
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public string? Color { get; set; }
    }
}