using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;

namespace DiscProfilesApi.Services.GraphServices
{
    public class GraphStressMeasureService
    {
        private readonly IDriver _driver;
        private readonly DiscProfilesContext _sqlContext;

        public GraphStressMeasureService(IDriver driver, DiscProfilesContext sqlContext)
        {
            _driver = driver;
            _sqlContext = sqlContext;
        }

        public async Task<IEnumerable<StressMeasureDetailDto>> GetStressMeasuresForEmployeeAsync(int employeeId)
        {
            var result = new List<StressMeasureDetailDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (e:Employee {id: $employeeId})-[:HAS_STRESS_MEASURE]->(sm:StressMeasure)
                OPTIONAL MATCH (sm)-[:FOR_TASK]->(t:Task)
                RETURN sm.id AS measureId, sm.measure AS measure, sm.description AS description, 
                       t.name AS taskName
                ORDER BY sm.id
            ", new { employeeId });

            await foreach (var record in cursor)
            {
                result.Add(new StressMeasureDetailDto
                {
                    MeasureId = record["measureId"].As<int>(),
                    Measure = record["measure"].As<int?>(),
                    Description = record["description"].As<string>(),
                    TaskName = record["taskName"].As<string>()
                });
            }

            return result;
        }

        public async Task MirrorStressMeasureFromSqlAsync(int measureId)
        {
            var measure = await _sqlContext.stress_measures
                .Include(s => s.employee)
                .Include(s => s.task)
                .FirstOrDefaultAsync(s => s.id == measureId);

            if (measure == null)
                return;

            await using var session = _driver.AsyncSession();

            const string measureCypher = @"
MERGE (sm:StressMeasure {id: $id})
SET 
    sm.description = $description,
    sm.measure = $measure
WITH sm
OPTIONAL MATCH (e:Employee {id: $employeeId})
FOREACH (_ IN CASE WHEN e IS NULL THEN [] ELSE [1] END |
    MERGE (e)-[:HAS_STRESS_MEASURE]->(sm)
)
WITH sm
OPTIONAL MATCH (t:Task {id: $taskId})
FOREACH (_ IN CASE WHEN t IS NULL THEN [] ELSE [1] END |
    MERGE (sm)-[:FOR_TASK]->(t)
);
";

            await session.RunAsync(measureCypher, new
            {
                id = measure.id,
                description = measure.description,
                measure = measure.measure,
                employeeId = measure.employee_id,
                taskId = measure.task_id
            });
        }

        // Updates the StressMeasure node with the given id
        public async Task<bool> UpdateStressMeasureNodeAsync(int id, string? description, int? measure)
        {
            await using var session = _driver.AsyncSession();

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?> { { "id", id } };

            if (!string.IsNullOrEmpty(description))
            {
                setClauses.Add("sm.description = $description");
                parameters["description"] = description;
            }

            if (measure.HasValue)
            {
                setClauses.Add("sm.measure = $measure");
                parameters["measure"] = measure;
            }

            if (setClauses.Count == 0)
                return false;

            var setClause = string.Join(", ", setClauses);

            var result = await session.RunAsync($@"
                MATCH (sm:StressMeasure {{id: $id}})
                SET {setClause}
            ", parameters);

            var summary = await result.ConsumeAsync();
            return summary.Counters.PropertiesSet > 0;
        }

        // Deletes the StressMeasure node with the given id
        public async Task<bool> DeleteStressMeasureNodeAsync(int id)
        {
            await using var session = _driver.AsyncSession();

            var result = await session.RunAsync(@"
                MATCH (sm:StressMeasure {id: $id})
                DETACH DELETE sm
            ", new { id });

            var summary = await result.ConsumeAsync();
            return summary.Counters.NodesDeleted > 0;
        }
    }

    public class StressMeasureDetailDto
    {
        public int MeasureId { get; set; }
        public int? Measure { get; set; }
        public string? Description { get; set; }
        public string? TaskName { get; set; }
    }
}