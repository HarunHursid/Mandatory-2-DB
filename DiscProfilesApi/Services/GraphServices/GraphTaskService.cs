using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;

namespace DiscProfilesApi.Services.GraphServices
{
    public class GraphTaskService
    {
        private readonly IDriver _driver;
        private readonly DiscProfilesContext _sqlContext;

        public GraphTaskService(IDriver driver, DiscProfilesContext sqlContext)
        {
            _driver = driver;
            _sqlContext = sqlContext;
        }

        public async Task<IEnumerable<TaskEmployeeDto>> GetEmployeesAssignedToTaskAsync(int taskId)
        {
            var result = new List<TaskEmployeeDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (t:Task {id: $taskId})<-[:ASSIGNED_TO_TASK]-(e:Employee)
                RETURN e.id AS employeeId, e.email AS email
                ORDER BY e.id
            ", new { taskId });

            await foreach (var record in cursor)
            {
                result.Add(new TaskEmployeeDto
                {
                    EmployeeId = record["employeeId"].As<int>(),
                    Email = record["email"].As<string>()
                });
            }

            return result;
        }

        public async Task MirrorTaskFromSqlAsync(int taskId)
        {
            var task = await _sqlContext.tasks
                .Include(t => t.project)
                .FirstOrDefaultAsync(t => t.id == taskId);

            if (task == null)
                return;

            await using var session = _driver.AsyncSession();

            const string taskCypher = @"
MERGE (t:Task {id: $id})
SET 
    t.name = $name,
    t.completed = $completed,
    t.timeOfCompletion = $timeOfCompletion
WITH t
MATCH (p:Project {id: $projectId})
MERGE (p)-[:HAS_TASK]->(t);
";

            await session.RunAsync(taskCypher, new
            {
                id = task.id,
                name = task.name,
                completed = task.completed,
                timeOfCompletion = task.time_of_completion,
                projectId = task.project_id
            });
        }

        // Updates the Task node with the given id
        public async Task<bool> UpdateTaskNodeAsync(int id, string? name, bool? completed)
        {
            await using var session = _driver.AsyncSession();

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?> { { "id", id } };

            if (!string.IsNullOrEmpty(name))
            {
                setClauses.Add("t.name = $name");
                parameters["name"] = name;
            }

            if (completed.HasValue)
            {
                setClauses.Add("t.completed = $completed");
                parameters["completed"] = completed;
            }

            if (setClauses.Count == 0)
                return false;

            var setClause = string.Join(", ", setClauses);

            var result = await session.RunAsync($@"
                MATCH (t:Task {{id: $id}})
                SET {setClause}
            ", parameters);

            var summary = await result.ConsumeAsync();
            return summary.Counters.PropertiesSet > 0;
        }

        // Deletes the Task node with the given id
        public async Task<bool> DeleteTaskNodeAsync(int id)
        {
            await using var session = _driver.AsyncSession();

            var result = await session.RunAsync(@"
                MATCH (t:Task {id: $id})
                DETACH DELETE t
            ", new { id });

            var summary = await result.ConsumeAsync();
            return summary.Counters.NodesDeleted > 0;
        }
    }

    public class TaskEmployeeDto
    {
        public int EmployeeId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}