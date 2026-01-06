using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;

namespace DiscProfilesApi.Services.GraphServices
{
    public class GraphDepartmentService
    {
        private readonly IDriver _driver;
        private readonly DiscProfilesContext _sqlContext;

        public GraphDepartmentService(IDriver driver, DiscProfilesContext sqlContext)
        {
            _driver = driver;
            _sqlContext = sqlContext;
        }

        // REN GRAPH: alle medarbejdere i et departement
        public async Task<IEnumerable<DepartmentEmployeeDto>> GetEmployeesInDepartmentAsync(int departmentId)
        {
            var result = new List<DepartmentEmployeeDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (d:Department {id: $departmentId})<-[:IN_DEPARTMENT]-(e:Employee)
                RETURN e.id AS employeeId, e.email AS email
                ORDER BY e.id
            ", new { departmentId });

            await foreach (var record in cursor)
            {
                result.Add(new DepartmentEmployeeDto
                {
                    EmployeeId = record["employeeId"].As<int>(),
                    Email = record["email"].As<string>()
                });
            }

            return result;
        }

        // SYNC: spejl department fra SQL til Neo4j
        public async Task MirrorDepartmentFromSqlAsync(int departmentId)
        {
            var department = await _sqlContext.departments
                .Include(d => d.company)
                .FirstOrDefaultAsync(d => d.id == departmentId);

            if (department == null)
                return;

            await using var session = _driver.AsyncSession();

            const string departmentCypher = @"
MERGE (d:Department {id: $id})
SET 
    d.name = $name
WITH d
MATCH (c:Company {id: $companyId})
MERGE (d)-[:PART_OF]->(c);
";

            await session.RunAsync(departmentCypher, new
            {
                id = department.id,
                name = department.name,
                companyId = department.company_id
            });
        }

        // Updates the Department node with the given id
        public async Task<bool> UpdateDepartmentNodeAsync(int id, string? name)
        {
            await using var session = _driver.AsyncSession();

            if (string.IsNullOrEmpty(name))
                return false;

            var result = await session.RunAsync(@"
                MATCH (d:Department {id: $id})
                SET d.name = $name
            ", new { id, name });

            var summary = await result.ConsumeAsync();
            return summary.Counters.PropertiesSet > 0;
        }

        // Deletes the Department node with the given id
        public async Task<bool> DeleteDepartmentNodeAsync(int id)
        {
            await using var session = _driver.AsyncSession();

            var result = await session.RunAsync(@"
                MATCH (d:Department {id: $id})
                DETACH DELETE d
            ", new { id });

            var summary = await result.ConsumeAsync();
            return summary.Counters.NodesDeleted > 0;
        }
    }

    public class DepartmentEmployeeDto
    {
        public int EmployeeId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}