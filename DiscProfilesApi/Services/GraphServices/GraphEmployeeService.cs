using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscProfilesApi.Services.GraphServices
{
    public class GraphEmployeeService
    {
        private readonly IDriver _driver;
        private readonly DiscProfilesContext _sqlContext;

        public GraphEmployeeService(IDriver driver, DiscProfilesContext sqlContext)
        {
            _driver = driver;         // Neo4j
            _sqlContext = sqlContext; // SQL
        }

        // 1) REN GRAPH: alle employees + deres company (via WORKS_FOR)
        public async Task<IEnumerable<EmployeeCompanyDto>> GetEmployeesWithCompanyAsync()
        {
            var result = new List<EmployeeCompanyDto>();

            await using var session = _driver.AsyncSession(); // default Neo4j-db

            var cursor = await session.RunAsync(@"
                MATCH (e:Employee)-[:WORKS_FOR]->(c:Company)
                RETURN e.id AS employeeId, c.name AS companyName
            ");

            await foreach (var record in cursor)
            {
                result.Add(new EmployeeCompanyDto
                {
                    EmployeeId = record["employeeId"].As<int>(),
                    CompanyName = record["companyName"].As<string>()
                });
            }

            return result;
        }



        public async Task<Dictionary<string, object>?> GetNodeByLabelAndIdAsync(string label, int id)
        {
            if (!Regex.IsMatch(label, "^[A-Za-z_][A-Za-z0-9_]*$"))
                throw new ArgumentException("Invalid label");

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync($@"
MATCH (n:{label} {{id: $id}})
RETURN properties(n) AS props
LIMIT 1
", new { id });

            var hasRecord = await cursor.FetchAsync();
            if (!hasRecord) return null;

            var record = cursor.Current;

            // props kan være Map/Dictionary alt efter driver - men As<Dictionary<string, object>>() plejer at virke
            return record["props"].As<Dictionary<string, object>>();
        }

        // 2) REN GRAPH: kolleger i samme company
        public async Task<IEnumerable<ColleagueDto>> GetColleaguesInSameCompanyAsync(int employeeId)
        {
            var result = new List<ColleagueDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (e:Employee {id: $employeeId})-[:WORKS_FOR]->(c:Company)<-[:WORKS_FOR]-(colleague:Employee)
                WHERE colleague.id <> e.id
                RETURN DISTINCT colleague.id AS colleagueId
            ", new { employeeId });

            await foreach (var record in cursor)
            {
                result.Add(new ColleagueDto
                {
                    EmployeeId = record["colleagueId"].As<int>()
                });
            }

            return result;
        }

        // X) REN GRAPH: projekter som en employee arbejder på
        public async Task<IEnumerable<EmployeeProjectDto>> GetProjectsForEmployeeAsync(int employeeId)
        {
            var result = new List<EmployeeProjectDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (e:Employee {id: $employeeId})-[:WORKS_ON]->(p:Project)
                RETURN e.id AS employeeId, p.id AS projectId, p.name AS projectName
            ", new { employeeId });

            await foreach (var record in cursor)
            {
                result.Add(new EmployeeProjectDto
                {
                    EmployeeId = record["employeeId"].As<int>(),
                    ProjectId = record["projectId"].As<int>(),
                    ProjectName = record["projectName"].As<string>()
                });
            }

            return result;
        }

        // X) REN GRAPH: tasks for en employee (via projekter)
        public async Task<IEnumerable<EmployeeTaskDto>> GetTasksForEmployeeAsync(int employeeId)
        {
            var result = new List<EmployeeTaskDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (e:Employee {id: $employeeId})-[:WORKS_ON]->(p:Project)-[:HAS_TASK]->(t:Task)
                RETURN e.id AS employeeId,
                       p.name AS projectName,
                       t.id AS taskId,
                       t.name AS taskName
            ", new { employeeId });

            await foreach (var record in cursor)
            {
                result.Add(new EmployeeTaskDto
                {
                    EmployeeId = record["employeeId"].As<int>(),
                    ProjectName = record["projectName"].As<string>(),
                    TaskId = record["taskId"].As<int>(),
                    TaskName = record["taskName"].As<string>()
                });
            }

            return result;
        }

        // 3) SQL + GRAPH SAMMEN:
        //    - SQL: hent employee + company-navn + email
        //    - GRAPH: hent projekter via WORKS_ON relation
        public async Task<EmployeeOverviewDto?> GetEmployeeOverviewAsync(int employeeId)
        {
            // SQL DELEN
            var employee = await _sqlContext.employees
                .Include(e => e.company)
                .FirstOrDefaultAsync(e => e.id == employeeId);

            if (employee == null)
            {
                return null;
            }

            var overview = new EmployeeOverviewDto
            {
                EmployeeId = employee.id,
                SqlCompanyName = employee.company?.name,
                SqlEmail = employee.email
            };

            // GRAPH DELEN
            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (e:Employee {id: $employeeId})-[:WORKS_ON]->(p:Project)
                RETURN DISTINCT p.name AS projectName
            ", new { employeeId });

            var projects = new List<string>();
            await foreach (var record in cursor)
            {
                projects.Add(record["projectName"].As<string>());
            }

            overview.GraphProjects = projects;

            return overview;
        }

        // 4) WRITE I GRAPH (CREATE)
        public async Task CreateEmployeeNodeAsync(int id, string email)
        {
            await using var session = _driver.AsyncSession();

            await session.RunAsync(@"
                CREATE (:Employee { id: $id, email: $email })
            ", new { id, email });
        }

        // 5) SYNC: spejl én employee + person + projekter + tasks fra SQL over i Neo4j
        public async Task MirrorEmployeeFromSqlAsync(int employeeId)
        {
            // Hent employee + relateret data fra SQL
            var e = await _sqlContext.employees
                .Include(x => x.company)
                .Include(x => x.department)
                .Include(x => x.person)
                .Include(x => x.projects)
                    .ThenInclude(p => p.tasks)   // projekter + deres tasks
                .FirstOrDefaultAsync(x => x.id == employeeId);

            if (e == null)
                return;

            await using var session = _driver.AsyncSession();

            // 1) PERSON-node (opret/merge ud fra SQL-person)
            if (e.person_id.HasValue && e.person != null)
            {
                const string personCypher = @"
MERGE (p:Person {id: $id})
SET 
    p.privateEmail = $privateEmail,
    p.privatePhone = $privatePhone,
    p.cpr = $cpr,
    p.firstName = $firstName,
    p.lastName = $lastName,
    p.experience = $experience,
    p.educationId = $educationId;
";

                await session.RunAsync(personCypher, new
                {
                    id = e.person_id.Value,
                    privateEmail = e.person.private_email,
                    privatePhone = e.person.private_phone,
                    e.person.cpr,
                    firstName = e.person.first_name,
                    lastName = e.person.last_name,
                    e.person.experience,
                    educationId = e.person.EducationID
                });
            }

            // 2) EMPLOYEE-node + relationer (WORKS_FOR / IN_DEPARTMENT / HAS_PERSON)
            const string employeeCypher = @"
MERGE (emp:Employee {id: $id})
SET 
    emp.email = $email,
    emp.phone = $phone
WITH emp
MATCH (c:Company {id: $companyId})
MERGE (emp)-[:WORKS_FOR]->(c)
WITH emp
OPTIONAL MATCH (d:Department {id: $departmentId})
FOREACH (_ IN CASE WHEN d IS NULL THEN [] ELSE [1] END |
    MERGE (emp)-[:IN_DEPARTMENT]->(d)
)
WITH emp
OPTIONAL MATCH (p:Person {id: $personId})
FOREACH (_ IN CASE WHEN p IS NULL THEN [] ELSE [1] END |
    MERGE (emp)-[:HAS_PERSON]->(p)
);
";

            await session.RunAsync(employeeCypher, new
            {
                e.id,
                e.email,
                e.phone,
                companyId = e.company_id,
                departmentId = e.department_id,
                personId = e.person_id
            });

            // 3) PROJEKTER + TASKS (WORKS_ON, HAS_TASK, ASSIGNED_TO_TASK)
            foreach (var proj in e.projects)
            {
                // Projektnode + relation Employee-[:WORKS_ON]->Project
                const string projectCypher = @"
MERGE (p:Project {id: $projectId})
SET 
    p.name = $name,
    p.description = $description,
    p.deadline = $deadline,
    p.completed = $completed,
    p.numberOfEmployees = $numberOfEmployees
WITH p
MATCH (emp:Employee {id: $employeeId})
MERGE (emp)-[:WORKS_ON]->(p);
";

                await session.RunAsync(projectCypher, new
                {
                    projectId = proj.id,
                    proj.name,
                    proj.description,
                    proj.deadline,
                    proj.completed,
                    numberOfEmployees = proj.number_of_employees,
                    employeeId = e.id
                });

                // Tasks under det projekt
                foreach (var t in proj.tasks)
                {
                    const string taskCypher = @"
MERGE (t:Task {id: $taskId})
SET 
    t.name = $name,
    t.completed = $completed,
    t.timeOfCompletion = $timeOfCompletion
WITH t
MATCH (p:Project {id: $projectId})
MERGE (p)-[:HAS_TASK]->(t)
WITH t
MATCH (emp:Employee {id: $employeeId})
MERGE (emp)-[:ASSIGNED_TO_TASK]->(t);
";

                    await session.RunAsync(taskCypher, new
                    {
                        taskId = t.id,
                        t.name,
                        t.completed,
                        timeOfCompletion = t.time_of_completion,
                        projectId = proj.id,
                        employeeId = e.id
                    });
                }
            }
        }

        // UPDATE: opdater employee node
        public async Task<bool> UpdateEmployeeNodeAsync(int id, string? email, string? phone)
        {
            await using var session = _driver.AsyncSession();

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?> { { "id", id } };

            if (!string.IsNullOrEmpty(email))
            {
                setClauses.Add("e.email = $email");
                parameters["email"] = email;
            }

            if (!string.IsNullOrEmpty(phone))
            {
                setClauses.Add("e.phone = $phone");
                parameters["phone"] = phone;
            }

            if (setClauses.Count == 0)
                return false;

            var setClause = string.Join(", ", setClauses);

            var result = await session.RunAsync($@"
                MATCH (e:Employee {{id: $id}})
                SET {setClause}
            ", parameters);

            var summary = await result.ConsumeAsync();
            return summary.Counters.PropertiesSet > 0;
        }

        // DELETE: slet employee node og alle relationer
        public async Task<bool> DeleteEmployeeNodeAsync(int id)
        {
            await using var session = _driver.AsyncSession();

            var result = await session.RunAsync(@"
        MATCH (e:Employee {id: $id})
        DETACH DELETE e
    ", new { id });

            var summary = await result.ConsumeAsync();
            return summary.Counters.NodesDeleted > 0;
        }
    }

    public class EmployeeCompanyDto
    {
        public int EmployeeId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }

    public class ColleagueDto
    {
        public int EmployeeId { get; set; }
    }

    public class EmployeeOverviewDto
    {
        public int EmployeeId { get; set; }
        public string? SqlCompanyName { get; set; }   // fra SQL
        public string? SqlEmail { get; set; }         // fra SQL
        public List<string> GraphProjects { get; set; } = new(); // fra Neo4j
    }

    public class EmployeeProjectDto
    {
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
    }

    public class EmployeeTaskDto
    {
        public int EmployeeId { get; set; }
        public int TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
    }

}
