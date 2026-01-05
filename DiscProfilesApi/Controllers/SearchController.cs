using DiscProfilesApi.Models;
using DiscProfilesApi.MongoDocuments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Neo4j.Driver;

namespace DiscProfilesApi.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : Controller
    {
        private readonly DiscProfilesContext _sql;
        private readonly IMongoDatabase _mongo;
        private readonly IDriver _neo;

        public SearchController(DiscProfilesContext sql, IMongoDatabase mongo, IDriver neo)
        {
            _sql = sql;
            _mongo = mongo;
            _neo = neo;
        }

        // ---------- SQL (all entities, text-ish fields) ----------
        // returns suggestions: { source, type, id, label }
        [HttpGet("sql")]
        public async Task<IActionResult> SearchSql([FromQuery] string q, [FromQuery] int limit = 10)
        {
            q = (q ?? string.Empty).Trim();
            if (q.Length < 2) return Ok(Array.Empty<object>());

            var like = $"%{q}%";
            var perTable = Math.Max(2, limit); // small slice per table

            var companies = await _sql.companies.AsNoTracking()
                .Where(x => EF.Functions.Like(x.name, like) ||
                            (x.location != null && EF.Functions.Like(x.location, like)) ||
                            (x.business_field != null && EF.Functions.Like(x.business_field, like)))
                .Select(x => new { source = "sql", type = "company", id = x.id, label = x.name })
                .Take(perTable).ToListAsync();

            var departments = await _sql.departments.AsNoTracking()
                .Where(x => EF.Functions.Like(x.name, like))
                .Select(x => new { source = "sql", type = "department", id = x.id, label = x.name })
                .Take(perTable).ToListAsync();

            var discProfiles = await _sql.disc_profiles.AsNoTracking()
                .Where(x => EF.Functions.Like(x.name, like) ||
                            (x.description != null && EF.Functions.Like(x.description, like)) ||
                            (x.color != null && EF.Functions.Like(x.color, like)))
                .Select(x => new { source = "sql", type = "disc_profile", id = x.id, label = x.name })
                .Take(perTable).ToListAsync();

            var educations = await _sql.educations.AsNoTracking()
                .Where(x => EF.Functions.Like(x.name, like) ||
                            (x.type != null && EF.Functions.Like(x.type, like)))
                .Select(x => new { source = "sql", type = "education", id = x.id, label = x.name })
                .Take(perTable).ToListAsync();

            var employees = await _sql.employees.AsNoTracking()
                .Where(x => (x.email != null && EF.Functions.Like(x.email, like)) ||
                            (x.phone != null && EF.Functions.Like(x.phone, like)))
                .Select(x => new { source = "sql", type = "employee", id = x.id, label = (x.email ?? x.phone ?? $"employee#{x.id}") })
                .Take(perTable).ToListAsync();

            var persons = await _sql.persons.AsNoTracking()
                .Where(x => EF.Functions.Like(x.first_name, like) ||
                            EF.Functions.Like(x.last_name, like) ||
                            EF.Functions.Like(x.cpr, like) ||
                            (x.private_email != null && EF.Functions.Like(x.private_email, like)) ||
                            (x.private_phone != null && EF.Functions.Like(x.private_phone, like)))
                .Select(x => new { source = "sql", type = "person", id = x.id, label = (x.first_name + " " + x.last_name).Trim() })
                .Take(perTable).ToListAsync();

            var positions = await _sql.positions.AsNoTracking()
                .Where(x => EF.Functions.Like(x.name, like))
                .Select(x => new { source = "sql", type = "position", id = x.id, label = x.name })
                .Take(perTable).ToListAsync();

            var projects = await _sql.projects.AsNoTracking()
                .Where(x => EF.Functions.Like(x.name, like) ||
                            (x.description != null && EF.Functions.Like(x.description, like)))
                .Select(x => new { source = "sql", type = "project", id = x.id, label = x.name })
                .Take(perTable).ToListAsync();

            var tasks = await _sql.tasks.AsNoTracking()
                .Where(x => EF.Functions.Like(x.name, like))
                .Select(x => new { source = "sql", type = "task", id = x.id, label = x.name })
                .Take(perTable).ToListAsync();

            var socialEvents = await _sql.social_events.AsNoTracking()
                .Where(x => EF.Functions.Like(x.name, like) ||
                            (x.description != null && EF.Functions.Like(x.description, like)))
                .Select(x => new { source = "sql", type = "social_event", id = x.id, label = x.name })
                .Take(perTable).ToListAsync();

            var stressMeasures = await _sql.stress_measures.AsNoTracking()
                .Where(x => x.description != null && EF.Functions.Like(x.description, like))
                .Select(x => new { source = "sql", type = "stress_measure", id = x.id, label = x.description! })
                .Take(perTable).ToListAsync();

            var taskEvaluations = await _sql.task_evaluations.AsNoTracking()
                .Where(x => x.description != null && EF.Functions.Like(x.description, like))
                .Select(x => new { source = "sql", type = "task_evaluation", id = x.id, label = x.description! })
                .Take(perTable).ToListAsync();

            var dailyTaskLogs = await _sql.daily_task_logs.AsNoTracking()
                .Where(x => x.time_to_finish != null && EF.Functions.Like(x.time_to_finish, like))
                .Select(x => new { source = "sql", type = "daily_task_log", id = x.id, label = x.time_to_finish! })
                .Take(perTable).ToListAsync();

            // projects_disc_profiles has no text columns, so we search by ids (as string) and create label after
            var projectsDiscProfiles = await _sql.projects_disc_profiles.AsNoTracking()
                .Where(x =>
                    EF.Functions.Like(x.project_id.ToString(), like) ||
                    EF.Functions.Like(x.disc_profile_id.ToString(), like))
                .Select(x => new
                {
                    source = "sql",
                    type = "projects_disc_profile",
                    id = x.id,
                    label = "project:" + x.project_id + " disc:" + x.disc_profile_id
                })
                .Take(perTable)
                .ToListAsync();

            var appUsers = await _sql.AppUsers.AsNoTracking()
                .Where(x => EF.Functions.Like(x.Email, like) || EF.Functions.Like(x.Role, like))
                .Select(x => new { source = "sql", type = "app_user", id = x.Id, label = x.Email })
                .Take(perTable).ToListAsync();

            var all =
                companies
                .Concat(departments)
                .Concat(discProfiles)
                .Concat(educations)
                .Concat(employees)
                .Concat(persons)
                .Concat(positions)
                .Concat(projects)
                .Concat(tasks)
                .Concat(socialEvents)
                .Concat(stressMeasures)
                .Concat(taskEvaluations)
                .Concat(dailyTaskLogs)
                .Concat(projectsDiscProfiles)
                .Concat(appUsers)
                .Take(limit)
                .ToList();

            return Ok(all);
        }

        // ---------- Mongo (companies only for now) ----------
        [HttpGet("mongo")]
        public async Task<IActionResult> SearchMongo([FromQuery] string q, [FromQuery] int limit = 10)
        {
            q = (q ?? string.Empty).Trim();
            if (q.Length < 2) return Ok(Array.Empty<object>());

            var col = _mongo.GetCollection<CompanyDocument>("companies");

            var filter = Builders<CompanyDocument>.Filter.Or(
                Builders<CompanyDocument>.Filter.Regex(x => x.Name, new BsonRegularExpression(q, "i")),
                Builders<CompanyDocument>.Filter.Regex(x => x.Location, new BsonRegularExpression(q, "i")),
                Builders<CompanyDocument>.Filter.Regex(x => x.BusinessField, new BsonRegularExpression(q, "i"))
            );

            var docs = await col.Find(filter).Limit(limit).ToListAsync();
            var result = docs.Select(x => new { source = "mongo", type = "company", id = x.Id, label = x.Name });
            return Ok(result);
        }

        // ---------- Neo4j (what you already have nodes for) ----------
        [HttpGet("neo")]
        public async Task<IActionResult> SearchNeo([FromQuery] string q, [FromQuery] int limit = 10)
        {
            q = (q ?? string.Empty).Trim();
            if (q.Length < 2) return Ok(Array.Empty<object>());

            await using var session = _neo.AsyncSession();

            var cursor = await session.RunAsync(@"
WITH toLower($q) AS q
MATCH (n)
WHERE (n:Company  AND n.name  IS NOT NULL AND toLower(n.name)  CONTAINS q)
   OR (n:Employee AND n.email IS NOT NULL AND toLower(n.email) CONTAINS q)
   OR (n:Project  AND n.name  IS NOT NULL AND toLower(n.name)  CONTAINS q)
   OR (n:Task     AND n.name  IS NOT NULL AND toLower(n.name)  CONTAINS q)
RETURN labels(n)[0] AS type, n.id AS id, coalesce(n.name, n.email, toString(n.id)) AS label
LIMIT $limit
", new { q, limit });

            var result = new List<object>();
            await foreach (var record in cursor)
            {
                result.Add(new
                {
                    source = "neo",
                    type = record["type"].As<string>(),
                    id = record["id"].As<int>(),
                    label = record["label"].As<string>()
                });
            }

            return Ok(result);
        }
    }
}

