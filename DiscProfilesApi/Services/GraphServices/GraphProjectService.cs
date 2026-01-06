using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;

namespace DiscProfilesApi.Services.GraphServices
{
    public class GraphProjectService
    {
        private readonly IDriver _driver;
        private readonly DiscProfilesContext _sqlContext;

        public GraphProjectService(IDriver driver, DiscProfilesContext sqlContext)
        {
            _driver = driver;
            _sqlContext = sqlContext;
        }

        public async Task<IEnumerable<ProjectTaskDto>> GetTasksInProjectAsync(int projectId)
        {
            var result = new List<ProjectTaskDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (p:Project {id: $projectId})-[:HAS_TASK]->(t:Task)
                RETURN t.id AS taskId, t.name AS taskName, t.completed AS completed
                ORDER BY t.name
            ", new { projectId });

            await foreach (var record in cursor)
            {
                result.Add(new ProjectTaskDto
                {
                    TaskId = record["taskId"].As<int>(),
                    TaskName = record["taskName"].As<string>(),
                    Completed = record["completed"].As<bool>()
                });
            }

            return result;
        }

        public async Task<IEnumerable<ProjectEmployeeDto>> GetEmployeesOnProjectAsync(int projectId)
        {
            var result = new List<ProjectEmployeeDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (p:Project {id: $projectId})<-[:WORKS_ON]-(e:Employee)
                RETURN e.id AS employeeId, e.email AS email
                ORDER BY e.id
            ", new { projectId });

            await foreach (var record in cursor)
            {
                result.Add(new ProjectEmployeeDto
                {
                    EmployeeId = record["employeeId"].As<int>(),
                    Email = record["email"].As<string>()
                });
            }

            return result;
        }

        public async Task MirrorProjectFromSqlAsync(int projectId)
        {
            var project = await _sqlContext.projects
                .FirstOrDefaultAsync(p => p.id == projectId);

            if (project == null)
                return;

            await using var session = _driver.AsyncSession();

            const string projectCypher = @"
MERGE (p:Project {id: $id})
SET 
    p.name = $name,
    p.description = $description,
    p.deadline = $deadline,
    p.completed = $completed,
    p.numberOfEmployees = $numberOfEmployees;
";

            await session.RunAsync(projectCypher, new
            {
                id = project.id,
                name = project.name,
                description = project.description,
                deadline = project.deadline,
                completed = project.completed,
                numberOfEmployees = project.number_of_employees
            });
        }

        // Updates the Project node with the given id
        public async Task<bool> UpdateProjectNodeAsync(int id, string? name, string? description, string? deadline, string? completed, int? numberOfEmployees)
        {
            await using var session = _driver.AsyncSession();

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?> { { "id", id } };

            if (!string.IsNullOrEmpty(name))
            {
                setClauses.Add("p.name = $name");
                parameters["name"] = name;
            }

            if (!string.IsNullOrEmpty(description))
            {
                setClauses.Add("p.description = $description");
                parameters["description"] = description;
            }

            if (!string.IsNullOrEmpty(deadline))
            {
                setClauses.Add("p.deadline = $deadline");
                parameters["deadline"] = deadline;
            }

            if (!string.IsNullOrEmpty(completed))
            {
                setClauses.Add("p.completed = $completed");
                parameters["completed"] = completed;
            }

            if (numberOfEmployees.HasValue)
            {
                setClauses.Add("p.numberOfEmployees = $numberOfEmployees");
                parameters["numberOfEmployees"] = numberOfEmployees;
            }

            if (setClauses.Count == 0)
                return false;

            var setClause = string.Join(", ", setClauses);

            var result = await session.RunAsync($@"
                MATCH (p:Project {{id: $id}})
                SET {setClause}
            ", parameters);

            var summary = await result.ConsumeAsync();
            return summary.Counters.PropertiesSet > 0;
        }

        // Deletes the Project node with the given id
        public async Task<bool> DeleteProjectNodeAsync(int id)
        {
            await using var session = _driver.AsyncSession();

            var result = await session.RunAsync(@"
                MATCH (p:Project {id: $id})
                DETACH DELETE p
            ", new { id });

            var summary = await result.ConsumeAsync();
            return summary.Counters.NodesDeleted > 0;
        }
    }

    public class ProjectTaskDto
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public bool Completed { get; set; }
    }

    public class ProjectEmployeeDto
    {
        public int EmployeeId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}