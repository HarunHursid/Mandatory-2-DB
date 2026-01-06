using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;

namespace DiscProfilesApi.Services.GraphServices
{
    public class GraphPersonService
    {
        private readonly IDriver _driver;
        private readonly DiscProfilesContext _sqlContext;

        public GraphPersonService(IDriver driver, DiscProfilesContext sqlContext)
        {
            _driver = driver;
            _sqlContext = sqlContext;
        }

        // REN GRAPH: alle employees knyttet til en person
        public async Task<IEnumerable<PersonEmployeeDto>> GetEmployeesForPersonAsync(int personId)
        {
            var result = new List<PersonEmployeeDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (p:Person {id: $personId})<-[:HAS_PERSON]-(e:Employee)
                RETURN e.id AS employeeId, e.email AS email
                ORDER BY e.id
            ", new { personId });

            await foreach (var record in cursor)
            {
                result.Add(new PersonEmployeeDto
                {
                    EmployeeId = record["employeeId"].As<int>(),
                    Email = record["email"].As<string>()
                });
            }

            return result;
        }

        // SYNC: spejl person fra SQL til Neo4j
        public async Task MirrorPersonFromSqlAsync(int personId)
        {
            var person = await _sqlContext.persons
                .FirstOrDefaultAsync(p => p.id == personId);

            if (person == null)
                return;

            await using var session = _driver.AsyncSession();

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
                id = person.id,
                privateEmail = person.private_email,
                privatePhone = person.private_phone,
                cpr = person.cpr,
                firstName = person.first_name,
                lastName = person.last_name,
                experience = person.experience,
                educationId = person.EducationID
            });
        }

        // Updates the Person node with the given id
        public async Task<bool> UpdatePersonNodeAsync(int id, string? firstName, string? lastName, int? experience)
        {
            await using var session = _driver.AsyncSession();

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?> { { "id", id } };

            if (!string.IsNullOrEmpty(firstName))
            {
                setClauses.Add("p.firstName = $firstName");
                parameters["firstName"] = firstName;
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                setClauses.Add("p.lastName = $lastName");
                parameters["lastName"] = lastName;
            }

            if (experience.HasValue)
            {
                setClauses.Add("p.experience = $experience");
                parameters["experience"] = experience;
            }

            if (setClauses.Count == 0)
                return false;

            var setClause = string.Join(", ", setClauses);

            var result = await session.RunAsync($@"
                MATCH (p:Person {{id: $id}})
                SET {setClause}
            ", parameters);

            var summary = await result.ConsumeAsync();
            return summary.Counters.PropertiesSet > 0;
        }

        // Deletes the Person node with the given id
        public async Task<bool> DeletePersonNodeAsync(int id)
        {
            await using var session = _driver.AsyncSession();

            var result = await session.RunAsync(@"
                MATCH (p:Person {id: $id})
                DETACH DELETE p
            ", new { id });

            var summary = await result.ConsumeAsync();
            return summary.Counters.NodesDeleted > 0;
        }
    }

    public class PersonEmployeeDto
    {
        public int EmployeeId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
