using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;

namespace DiscProfilesApi.Services.GraphServices
{
    public class GraphTaskEvaluationService
    {
        private readonly IDriver _driver;
        private readonly DiscProfilesContext _sqlContext;

        public GraphTaskEvaluationService(IDriver driver, DiscProfilesContext sqlContext)
        {
            _driver = driver;
            _sqlContext = sqlContext;
        }

        // REN GRAPH: alle evaluations for en task
        public async Task<IEnumerable<TaskEvaluationDetailDto>> GetEvaluationsForTaskAsync(int taskId)
        {
            var result = new List<TaskEvaluationDetailDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (t:Task {id: $taskId})<-[:EVALUATES]-(te:TaskEvaluation)
                RETURN te.id AS evaluationId, te.description AS description, te.difficultyRange AS difficultyRange
                ORDER BY te.id
            ", new { taskId });

            await foreach (var record in cursor)
            {
                result.Add(new TaskEvaluationDetailDto
                {
                    EvaluationId = record["evaluationId"].As<int>(),
                    Description = record["description"].As<string>(),
                    DifficultyRange = record["difficultyRange"].As<int?>()
                });
            }

            return result;
        }

        // UPDATE
        public async Task<bool> UpdateTaskEvaluationNodeAsync(int id, string? description, int? difficultyRange)
        {
            await using var session = _driver.AsyncSession();

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?> { { "id", id } };

            if (!string.IsNullOrEmpty(description))
            {
                setClauses.Add("te.description = $description");
                parameters["description"] = description;
            }

            if (difficultyRange.HasValue)
            {
                setClauses.Add("te.difficultyRange = $difficultyRange");
                parameters["difficultyRange"] = difficultyRange;
            }

            if (setClauses.Count == 0)
                return false;

            var setClause = string.Join(", ", setClauses);

            var result = await session.RunAsync($@"
                MATCH (te:TaskEvaluation {{id: $id}})
                SET {setClause}
            ", parameters);

            var summary = await result.ConsumeAsync();
            return summary.Counters.PropertiesSet > 0;
        }

        // DELETE
        public async Task<bool> DeleteTaskEvaluationNodeAsync(int id)
        {
            await using var session = _driver.AsyncSession();

            var result = await session.RunAsync(@"
                MATCH (te:TaskEvaluation {id: $id})
                DETACH DELETE te
            ", new { id });

            var summary = await result.ConsumeAsync();
            return summary.Counters.NodesDeleted > 0;
        }

        // SYNC
        public async Task MirrorTaskEvaluationFromSqlAsync(int evaluationId)
        {
            var evaluation = await _sqlContext.task_evaluations
                .Include(e => e.task)
                .FirstOrDefaultAsync(e => e.id == evaluationId);

            if (evaluation == null)
                return;

            await using var session = _driver.AsyncSession();

            const string evaluationCypher = @"
MERGE (te:TaskEvaluation {id: $id})
SET 
    te.description = $description,
    te.difficultyRange = $difficultyRange
WITH te
MATCH (t:Task {id: $taskId})
MERGE (te)-[:EVALUATES]->(t);
";

            await session.RunAsync(evaluationCypher, new
            {
                id = evaluation.id,
                description = evaluation.description,
                difficultyRange = evaluation.difficulty_range,
                taskId = evaluation.task_id
            });
        }
    }

    public class TaskEvaluationDetailDto
    {
        public int EvaluationId { get; set; }
        public string? Description { get; set; }
        public int? DifficultyRange { get; set; }
    }
}