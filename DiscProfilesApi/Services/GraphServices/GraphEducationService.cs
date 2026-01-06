using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscProfilesApi.Models;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;

namespace DiscProfilesApi.Services.GraphServices
{
    public class GraphEducationService
    {
        private readonly IDriver _driver;
        private readonly DiscProfilesContext _sqlContext;

        public GraphEducationService(IDriver driver, DiscProfilesContext sqlContext)
        {
            _driver = driver;
            _sqlContext = sqlContext;
        }

        // REN GRAPH: alle personer med en education
        public async Task<IEnumerable<EducationPersonDto>> GetPersonsWithEducationAsync(int educationId)
        {
            var result = new List<EducationPersonDto>();

            await using var session = _driver.AsyncSession();

            var cursor = await session.RunAsync(@"
                MATCH (ed:Education {id: $educationId})<-[:HAS_EDUCATION]-(p:Person)
                RETURN p.id AS personId, p.firstName AS firstName, p.lastName AS lastName
                ORDER BY p.firstName
            ", new { educationId });

            await foreach (var record in cursor)
            {
                result.Add(new EducationPersonDto
                {
                    PersonId = record["personId"].As<int>(),
                    FirstName = record["firstName"].As<string>(),
                    LastName = record["lastName"].As<string>()
                });
            }

            return result;
        }

        // SYNC: spejl education fra SQL til Neo4j
        public async Task MirrorEducationFromSqlAsync(int educationId)
        {
            var education = await _sqlContext.educations
                .FirstOrDefaultAsync(e => e.id == educationId);

            if (education == null)
                return;

            await using var session = _driver.AsyncSession();

            const string educationCypher = @"
MERGE (ed:Education {id: $id})
SET 
    ed.name = $name,
    ed.type = $type,
    ed.grade = $grade;
";

            await session.RunAsync(educationCypher, new
            {
                id = education.id,
                name = education.name,
                type = education.type,
                grade = education.grade
            });
        }

        // Updates the Education node with the given id
        public async Task<bool> UpdateEducationNodeAsync(int id, string? name, string? type, int? grade)
        {
            await using var session = _driver.AsyncSession();

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object?> { { "id", id } };

            if (!string.IsNullOrEmpty(name))
            {
                setClauses.Add("ed.name = $name");
                parameters["name"] = name;
            }

            if (!string.IsNullOrEmpty(type))
            {
                setClauses.Add("ed.type = $type");
                parameters["type"] = type;
            }

            if (grade.HasValue)
            {
                setClauses.Add("ed.grade = $grade");
                parameters["grade"] = grade;
            }

            if (setClauses.Count == 0)
                return false;

            var setClause = string.Join(", ", setClauses);

            var result = await session.RunAsync($@"
                MATCH (ed:Education {{id: $id}})
                SET {setClause}
            ", parameters);

            var summary = await result.ConsumeAsync();
            return summary.Counters.PropertiesSet > 0;
        }

        // Deletes the Education node with the given id
        public async Task<bool> DeleteEducationNodeAsync(int id)
        {
            await using var session = _driver.AsyncSession();

            var result = await session.RunAsync(@"
                MATCH (ed:Education {id: $id})
                DETACH DELETE ed
            ", new { id });

            var summary = await result.ConsumeAsync();
            return summary.Counters.NodesDeleted > 0;
        }
    }

    public class EducationPersonDto
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}