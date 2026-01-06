using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;

namespace DiscProfilesApi.Services.GraphServices
{
    public class GraphPositionService
    {
        private readonly IDriver _driver;
        private readonly DiscProfilesContext _sqlContext;

        public GraphPositionService(IDriver driver, DiscProfilesContext sqlContext)
        {
            _driver = driver;
            _sqlContext = sqlContext;
        }

        // REN GRAPH: alle employees med en stilling
        public async Task<IEnumerable<PositionEmployeeDto>> GetEmployeesWithPositionAsync(int positionId)
        {
            var result = new List<PositionEmployeeDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (po:Position {id: $positionId})<-[:HAS_POSITION]-(e:Employee)
                RETURN e.id AS employeeId, e.email AS email
                ORDER BY e.id
            ", new { positionId });

            await foreach (var record in cursor)
            {
                result.Add(new PositionEmployeeDto
                {
                    EmployeeId = record["employeeId"].As<int>(),
                    Email = record["email"].As<string>()
                });
            }

            return result;
        }

        // SYNC: spejl position fra SQL til Neo4j
        public async Task<int> MirrorPositionFromSqlAsync(int positionId)
        {
            var position = await _sqlContext.positions
                .FirstOrDefaultAsync(p => p.id == positionId);

            if (position == null)
                return 0;

            await using var session = _driver.AsyncSession();

            // 1️⃣ Sync selve Position noden
            const string positionCypher = @"
MERGE (po:Position {id: $id})
SET po.name = $name;
";

            await session.RunAsync(positionCypher, new
            {
                id = position.id,
                name = position.name
            });

            // 2️⃣ Find employees i SQL med denne position
            var employeeIds = await _sqlContext.employees
                .Where(e => e.position_id == positionId)   // <- evt. PositionID
                .Select(e => e.id)
                .ToListAsync();

            if (!employeeIds.Any())
                return 0;

            // 3️⃣ Sync relationerne i Neo4j
            var relationCypher = @"
UNWIND $employeeIds AS empId
MERGE (e:Employee {id: empId})
MERGE (po:Position {id: $positionId})
WITH e, po
OPTIONAL MATCH (e)-[r:HAS_POSITION]->(:Position)
DELETE r
MERGE (e)-[:HAS_POSITION]->(po);
";

            var result = await session.RunAsync(relationCypher, new { employeeIds, positionId });
            var summary = await result.ConsumeAsync();

            return summary.Counters.RelationshipsCreated;
        }

        // Updates the Position node with the given id
        public async Task<bool> UpdatePositionNodeAsync(int id, string? name)
        {
            await using var session = _driver.AsyncSession();

            if (string.IsNullOrEmpty(name))
                return false;

            var result = await session.RunAsync(@"
                MATCH (po:Position {id: $id})
                SET po.name = $name
            ", new { id, name });

            var summary = await result.ConsumeAsync();
            return summary.Counters.PropertiesSet > 0;
        }

        // Deletes the Position node with the given id
        public async Task<bool> DeletePositionNodeAsync(int id)
        {
            await using var session = _driver.AsyncSession();

            var result = await session.RunAsync(@"
                MATCH (po:Position {id: $id})
                DETACH DELETE po
            ", new { id });

            var summary = await result.ConsumeAsync();
            return summary.Counters.NodesDeleted > 0;
        }
    }

    public class PositionEmployeeDto
    {
        public int EmployeeId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}