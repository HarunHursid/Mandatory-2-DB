using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;

namespace DiscProfilesApi.Services.GraphServices
{
    public class GraphCompanyService
    {
        private readonly IDriver _driver;
        private readonly DiscProfilesContext _sqlContext;

        public GraphCompanyService(IDriver driver, DiscProfilesContext sqlContext)
        {
            _driver = driver;
            _sqlContext = sqlContext;
        }

        public async Task<IEnumerable<CompanyEmployeeCountDto>> GetCompaniesWithEmployeeCountAsync()
        {
            var result = new List<CompanyEmployeeCountDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (c:Company)
                OPTIONAL MATCH (c)<-[:WORKS_FOR]-(e:Employee)
                RETURN c.id AS companyId, c.name AS companyName, COUNT(e) AS employeeCount
                ORDER BY c.name
            ");

            await foreach (var record in cursor)
            {
                result.Add(new CompanyEmployeeCountDto
                {
                    CompanyId = record["companyId"].As<int>(),
                    CompanyName = record["companyName"].As<string>(),
                    EmployeeCount = record["employeeCount"].As<int>()
                });
            }
                    
            return result;
        }

        public async Task<IEnumerable<CompanyDepartmentDto>> GetDepartmentsInCompanyAsync(int companyId)
        {
            var result = new List<CompanyDepartmentDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (c:Company {id: $companyId})<-[:PART_OF]-(d:Department)
                RETURN d.id AS departmentId, d.name AS departmentName
            ", new { companyId });

            await foreach (var record in cursor)
            {
                result.Add(new CompanyDepartmentDto
                {
                    DepartmentId = record["departmentId"].As<int>(),
                    DepartmentName = record["departmentName"].As<string>()
                });
            }

            return result;
        }

        public async Task MirrorCompanyFromSqlAsync(int companyId)
        {
            var company = await _sqlContext.companies
                .FirstOrDefaultAsync(c => c.id == companyId);

            if (company == null)
                return;

            await using var session = _driver.AsyncSession();

            const string companyCypher = @"
MERGE (c:Company {id: $id})
SET 
    c.name = $name,
    c.location = $location,
    c.businessField = $businessField;
";

            await session.RunAsync(companyCypher, new
            {
                id = company.id,
                name = company.name,
                location = company.location,
                businessField = company.business_field
            });
        }

        // Updates the Company node with the given id
        public async Task<bool> UpdateCompanyNodeAsync(int id, string? name, string? location, string? businessField)
        {
            await using var session = _driver.AsyncSession();

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?> { { "id", id } };

            if (!string.IsNullOrEmpty(name))
            {
                setClauses.Add("c.name = $name");
                parameters["name"] = name;
            }

            if (!string.IsNullOrEmpty(location))
            {
                setClauses.Add("c.location = $location");
                parameters["location"] = location;
            }

            if (!string.IsNullOrEmpty(businessField))
            {
                setClauses.Add("c.businessField = $businessField");
                parameters["businessField"] = businessField;
            }

            if (setClauses.Count == 0)
                return false;

            var setClause = string.Join(", ", setClauses);

            var result = await session.RunAsync($@"
                MATCH (c:Company {{id: $id}})
                SET {setClause}
            ", parameters);

            var summary = await result.ConsumeAsync();
            return summary.Counters.PropertiesSet > 0;
        }

        // Deletes the Company node with the given id
        public async Task<bool> DeleteCompanyNodeAsync(int id)
        {
            await using var session = _driver.AsyncSession();

            var result = await session.RunAsync(@"
                MATCH (c:Company {id: $id})
                DETACH DELETE c
            ", new { id });

            var summary = await result.ConsumeAsync();
            return summary.Counters.NodesDeleted > 0;
        }
    }

    public class CompanyEmployeeCountDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
    }

    public class CompanyDepartmentDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }
}