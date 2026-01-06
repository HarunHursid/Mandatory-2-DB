using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;

namespace DiscProfilesApi.Services.GraphServices
{
    public class GraphDiscProfileService
    {
        private readonly IDriver _driver;
        private readonly DiscProfilesContext _sqlContext;

        public GraphDiscProfileService(IDriver driver, DiscProfilesContext sqlContext)
        {
            _driver = driver;
            _sqlContext = sqlContext;
        }

        public async Task<IEnumerable<DiscProfileEmployeeDto>> GetEmployeesWithDiscProfileAsync(int discProfileId)
        {
            var result = new List<DiscProfileEmployeeDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (dp:DiscProfile {id: $discProfileId})<-[:HAS_PROFILE]-(e:Employee)
                RETURN e.id AS employeeId, e.email AS email
                ORDER BY e.id
            ", new { discProfileId });

            await foreach (var record in cursor)
            {
                result.Add(new DiscProfileEmployeeDto
                {
                    EmployeeId = record["employeeId"].As<int>(),
                    Email = record["email"].As<string>()
                });
            }

            return result;
        }

        public async Task<IEnumerable<DiscProfileProjectDto>> GetProjectsWithDiscProfileAsync(int discProfileId)
        {
            var result = new List<DiscProfileProjectDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (dp:DiscProfile {id: $discProfileId})<-[:HAS_PROFILE]-(p:Project)
                RETURN p.id AS projectId, p.name AS projectName
                ORDER BY p.name
            ", new { discProfileId });

            await foreach (var record in cursor)
            {
                result.Add(new DiscProfileProjectDto
                {
                    ProjectId = record["projectId"].As<int>(),
                    ProjectName = record["projectName"].As<string>()
                });
            }

            return result;
        }

        public async Task MirrorDiscProfileFromSqlAsync(int discProfileId)
        {
            var profile = await _sqlContext.disc_profiles
                .FirstOrDefaultAsync(p => p.id == discProfileId);

            if (profile == null)
                return;

            await using var session = _driver.AsyncSession();

            const string profileCypher = @"
MERGE (dp:DiscProfile {id: $id})
SET 
    dp.name = $name,
    dp.color = $color,
    dp.description = $description;
";

            await session.RunAsync(profileCypher, new
            {
                id = profile.id,
                name = profile.name,
                color = profile.color,
                description = profile.description
            });
        }

        // Updates the DiscProfile node with the given id
        public async Task<bool> UpdateDiscProfileNodeAsync(int id, string? name, string? color, string? description)
        {
            await using var session = _driver.AsyncSession();

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?> { { "id", id } };

            if (!string.IsNullOrEmpty(name))
            {
                setClauses.Add("dp.name = $name");
                parameters["name"] = name;
            }

            if (!string.IsNullOrEmpty(color))
            {
                setClauses.Add("dp.color = $color");
                parameters["color"] = color;
            }

            if (!string.IsNullOrEmpty(description))
            {
                setClauses.Add("dp.description = $description");
                parameters["description"] = description;
            }

            if (setClauses.Count == 0)
                return false;

            var setClause = string.Join(", ", setClauses);

            var result = await session.RunAsync($@"
                MATCH (dp:DiscProfile {{id: $id}})
                SET {setClause}
            ", parameters);

            var summary = await result.ConsumeAsync();
            return summary.Counters.PropertiesSet > 0;
        }

        // Deletes the DiscProfile node with the given id
        public async Task<bool> DeleteDiscProfileNodeAsync(int id)
        {
            await using var session = _driver.AsyncSession();

            var result = await session.RunAsync(@"
                MATCH (dp:DiscProfile {id: $id})
                DETACH DELETE dp
            ", new { id });

            var summary = await result.ConsumeAsync();
            return summary.Counters.NodesDeleted > 0;
        }
    }

    public class DiscProfileEmployeeDto
    {
        public int EmployeeId { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    public class DiscProfileProjectDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
    }
}