using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;

namespace DiscProfilesApi.Services.GraphServices
{
    public class GraphSocialEventService
    {
        private readonly IDriver _driver;
        private readonly DiscProfilesContext _sqlContext;

        public GraphSocialEventService(IDriver driver, DiscProfilesContext sqlContext)
        {
            _driver = driver;
            _sqlContext = sqlContext;
        }

        // REN GRAPH: alle employees på et social event
        public async Task<IEnumerable<SocialEventEmployeeDto>> GetEmployeesAtEventAsync(int eventId)
        {
            var result = new List<SocialEventEmployeeDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (se:SocialEvent {id: $eventId})<-[:ATTENDS]-(e:Employee)
                RETURN e.id AS employeeId, e.email AS email
                ORDER BY e.id
            ", new { eventId });

            await foreach (var record in cursor)
            {
                result.Add(new SocialEventEmployeeDto
                {
                    EmployeeId = record["employeeId"].As<int>(),
                    Email = record["email"].As<string>()
                });
            }

            return result;
        }

        // SYNC: spejl social event fra SQL til Neo4j
        public async Task MirrorSocialEventFromSqlAsync(int eventId)
        {
            var socialEvent = await _sqlContext.social_events
                .Include(s => s.company)
                .Include(s => s.disc_profile)
                .FirstOrDefaultAsync(s => s.id == eventId);

            if (socialEvent == null)
                return;

            await using var session = _driver.AsyncSession();

            const string eventCypher = @"
MERGE (se:SocialEvent {id: $id})
SET 
    se.name = $name,
    se.description = $description
WITH se
OPTIONAL MATCH (c:Company {id: $companyId})
FOREACH (_ IN CASE WHEN c IS NULL THEN [] ELSE [1] END |
    MERGE (se)-[:AT_COMPANY]->(c)
)
WITH se
OPTIONAL MATCH (dp:DiscProfile {id: $discProfileId})
FOREACH (_ IN CASE WHEN dp IS NULL THEN [] ELSE [1] END |
    MERGE (se)-[:HAS_PROFILE]->(dp)
);
";

            await session.RunAsync(eventCypher, new
            {
                id = socialEvent.id,
                name = socialEvent.name,
                description = socialEvent.description,
                companyId = socialEvent.company_id,
                discProfileId = socialEvent.disc_profile_id
            });
        }

        // Updates the SocialEvent node with the given id
        public async Task<bool> UpdateSocialEventNodeAsync(int id, string? name, string? description)
        {
            await using var session = _driver.AsyncSession();

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?> { { "id", id } };

            if (!string.IsNullOrEmpty(name))
            {
                setClauses.Add("se.name = $name");
                parameters["name"] = name;
            }

            if (!string.IsNullOrEmpty(description))
            {
                setClauses.Add("se.description = $description");
                parameters["description"] = description;
            }

            if (setClauses.Count == 0)
                return false;

            var setClause = string.Join(", ", setClauses);

            var result = await session.RunAsync($@"
                MATCH (se:SocialEvent {{id: $id}})
                SET {setClause}
            ", parameters);

            var summary = await result.ConsumeAsync();
            return summary.Counters.PropertiesSet > 0;
        }

        // Deletes the SocialEvent node with the given id
        public async Task<bool> DeleteSocialEventNodeAsync(int id)
        {
            await using var session = _driver.AsyncSession();

            var result = await session.RunAsync(@"
                MATCH (se:SocialEvent {id: $id})
                DETACH DELETE se
            ", new { id });

            var summary = await result.ConsumeAsync();
            return summary.Counters.NodesDeleted > 0;
        }
    }

    public class SocialEventEmployeeDto
    {
        public int EmployeeId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}