using System;
using System.Threading.Tasks;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;

// TODO: ret namespace hvis dine EF-klasser ligger et andet sted
using DiscProfilesApi.Models;

internal partial class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Starting SQL → Neo4j graph migration...");

        // 1) Load .env
        var envPath = System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".env");
        Env.Load(envPath);

        var sqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
        var neo4jUri = Environment.GetEnvironmentVariable("NEO4J_URI");
        var neo4jUser = Environment.GetEnvironmentVariable("NEO4J_USER");
        var neo4jPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD");

        if (string.IsNullOrWhiteSpace(sqlConnectionString) ||
            string.IsNullOrWhiteSpace(neo4jUri) ||
            string.IsNullOrWhiteSpace(neo4jUser) ||
            string.IsNullOrWhiteSpace(neo4jPassword))
        {
            Console.WriteLine("❌ Missing one or more env vars. Check .env");
            return;
        }

        // 2) Setup EF Core DbContext
        var optionsBuilder = new DbContextOptionsBuilder<DiscProfilesContext>();
        optionsBuilder.UseSqlServer(sqlConnectionString);
        await using var sqlContext = new DiscProfilesContext(optionsBuilder.Options);

        // 3) Setup Neo4j driver
        IDriver driver = GraphDatabase.Driver(neo4jUri, AuthTokens.Basic(neo4jUser, neo4jPassword));

        await using var asyncSession = driver.AsyncSession();

        try
        {
            // Optional: Ryd HELE graphen først
            Console.WriteLine("Clearing Neo4j graph...");
            await asyncSession.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync("MATCH (n) DETACH DELETE n");
            });

            // 4) Migrér i rækkefølge (nodes først, relationer bagefter)
            await MigrateCompaniesAsync(sqlContext, asyncSession);
            await MigrateDepartmentsAsync(sqlContext, asyncSession);
            await MigratePersonsAsync(sqlContext, asyncSession);
            await MigrateEmployeesAsync(sqlContext, asyncSession);
            await MigrateProjectsAsync(sqlContext, asyncSession);
            await MigrateTasksAsync(sqlContext, asyncSession);
            //await MigrateEmployeesProjectsAsync(sqlContext, asyncSession);
            //await MigrateTasksEmployeesAsync(sqlContext, asyncSession);

            Console.WriteLine("✅ Graph migration completed.");
        }
        finally
        {
            await asyncSession.CloseAsync();
            await driver.DisposeAsync();
        }
    }

    // ----------------- COMPANIES → (:Company) -----------------

    private static async Task MigrateCompaniesAsync(DiscProfilesContext sql, IAsyncSession session)
    {
        Console.WriteLine("Migrating companies...");

        var rows = await sql.companies.ToListAsync();

        const string cypher = @"
MERGE (c:Company {id: $id})
SET c.name = $name,
    c.location = $location,
    c.businessField = $businessField";

        await session.ExecuteWriteAsync(async tx =>
        {
            foreach (var c in rows)
            {
                await tx.RunAsync(cypher, new
                {
                    id = c.id,
                    name = c.name,
                    location = c.location,
                    businessField = c.business_field
                });
            }
        });

        Console.WriteLine($"  → {rows.Count} companies migrated.");
    }

    // ----------------- DEPARTMENTS → (:Department)-[:BELONGS_TO]->(:Company) -----------------

    private static async Task MigrateDepartmentsAsync(DiscProfilesContext sql, IAsyncSession session)
    {
        Console.WriteLine("Migrating departments...");

        var rows = await sql.departments.ToListAsync();

        const string cypher = @"
MERGE (d:Department {id: $id})
SET d.name = $name
WITH d
MATCH (c:Company {id: $companyId})
MERGE (d)-[:BELONGS_TO]->(c);";

        await session.ExecuteWriteAsync(async tx =>
        {
            foreach (var d in rows)
            {
                await tx.RunAsync(cypher, new
                {
                    id = d.id,
                    name = d.name,
                    companyId = d.company_id
                });
            }
        });

        Console.WriteLine($"  → {rows.Count} departments migrated.");
    }

    // ----------------- PERSONS → (:Person) -----------------

    private static async Task MigratePersonsAsync(DiscProfilesContext sql, IAsyncSession session)
    {
        Console.WriteLine("Migrating persons...");

        var rows = await sql.persons.ToListAsync();

        const string cypher = @"
MERGE (p:Person {id: $id})
SET p.privateEmail = $privateEmail,
    p.privatePhone = $privatePhone,
    p.cpr = $cpr,
    p.firstName = $firstName,
    p.lastName = $lastName,
    p.experience = $experience,
    p.educationId = $educationId;";

        await session.ExecuteWriteAsync(async tx =>
        {
            foreach (var p in rows)
            {
                await tx.RunAsync(cypher, new
                {
                    id = p.id,
                    privateEmail = p.private_email,
                    privatePhone = p.private_phone,
                    cpr = p.cpr,
                    firstName = p.first_name,
                    lastName = p.last_name,
                    experience = p.experience,
                    educationId = p.EducationID
                });
            }
        });

        Console.WriteLine($"  → {rows.Count} persons migrated.");
    }

    // ----------------- EMPLOYEES → (:Employee)-relations -----------------

    private static async Task MigrateEmployeesAsync(DiscProfilesContext sql, IAsyncSession session)
    {
        Console.WriteLine("Migrating employees...");

        var rows = await sql.employees.ToListAsync();

        const string cypher = @"
MERGE (e:Employee {id: $id})
SET 
    e.email = $email,
    e.phone = $phone
WITH e
MATCH (c:Company {id: $companyId})
MERGE (e)-[:WORKS_FOR]->(c)
WITH e
OPTIONAL MATCH (d:Department {id: $departmentId})
FOREACH (_ IN CASE WHEN d IS NULL THEN [] ELSE [1] END |
    MERGE (e)-[:IN_DEPARTMENT]->(d)
)
WITH e
OPTIONAL MATCH (p:Person {id: $personId})
FOREACH (_ IN CASE WHEN p IS NULL THEN [] ELSE [1] END |
    MERGE (e)-[:HAS_PERSON]->(p)
);";

        await session.ExecuteWriteAsync(async tx =>
        {
            foreach (var e in rows)
            {
                await tx.RunAsync(cypher, new
                {
                    id = e.id,
                    email = e.email,
                    phone = e.phone,
                    companyId = e.company_id,
                    departmentId = e.department_id,
                    personId = e.person_id
                });
            }
        });

        Console.WriteLine($"  → {rows.Count} employees migrated.");
    }

    // ----------------- PROJECTS → (:Project) -----------------

    private static async Task MigrateProjectsAsync(DiscProfilesContext sql, IAsyncSession session)
    {
        Console.WriteLine("Migrating projects...");

        var rows = await sql.projects.ToListAsync();

        const string cypher = @"
MERGE (p:Project {id: $id})
SET p.name = $name,
    p.description = $description,
    p.deadline = $deadline,
    p.completed = $completed,
    p.numberOfEmployees = $numberOfEmployees;";

        await session.ExecuteWriteAsync(async tx =>
        {
            foreach (var p in rows)
            {
                await tx.RunAsync(cypher, new
                {
                    id = p.id,
                    name = p.name,
                    description = p.description,
                    deadline = p.deadline,
                    completed = p.completed,
                    numberOfEmployees = p.number_of_employees
                });
            }
        });

        Console.WriteLine($"  → {rows.Count} projects migrated.");
    }

    // ----------------- TASKS → (:Task)-[:HAS_TASK]->(:Project) -----------------

    private static async Task MigrateTasksAsync(DiscProfilesContext sql, IAsyncSession session)
    {
        Console.WriteLine("Migrating tasks...");

        var rows = await sql.tasks.ToListAsync();

        const string cypher = @"
MERGE (t:Task {id: $id})
SET t.name = $name,
    t.completed = $completed,
    t.timeOfCompletion = $timeOfCompletion
WITH t
MATCH (p:Project {id: $projectId})
MERGE (p)-[:HAS_TASK]->(t);";

        await session.ExecuteWriteAsync(async tx =>
        {
            foreach (var t in rows)
            {
                await tx.RunAsync(cypher, new
                {
                    id = t.id,
                    name = t.name,
                    completed = t.completed,
                    timeOfCompletion = t.time_of_completion,
                    projectId = t.project_id
                });
            }
        });

        Console.WriteLine($"  → {rows.Count} tasks migrated.");
    }

   /* // ----------------- EMPLOYEES_PROJECTS → (:Employee)-[:WORKS_ON]->(:Project) -----------------

    private static async Task MigrateEmployeesProjectsAsync(DiscProfilesContext sql, IAsyncSession session)
    {
        Console.WriteLine("Migrating employees_projects relationships...");

        var rows = await sql.EmployeesProjects.ToListAsync();

        const string cypher = @"
MATCH (e:Employee {id: $employeeId})
MATCH (p:Project {id: $projectId})
MERGE (e)-[:WORKS_ON]->(p);";

        await session.ExecuteWriteAsync(async tx =>
        {
            foreach (var ep in rows)
            {
                await tx.RunAsync(cypher, new
                {
                    employeeId = ep.EmployeeId,
                    projectId = ep.ProjectId
                });
            }
        });

        Console.WriteLine($"  → {rows.Count} employee-project relationships migrated.");
    }
   */
   /* // ----------------- TASKS_EMPLOYEES → (:Employee)-[:ASSIGNED_TO_TASK]->(:Task) -----------------

    private static async Task MigrateTasksEmployeesAsync(DiscProfilesContext sql, IAsyncSession session)
    {
        Console.WriteLine("Migrating tasks_employees relationships...");

        var rows = await sql.TasksEmployees.ToListAsync();

        const string cypher = @"
MATCH (e:Employee {id: $employeeId})
MATCH (t:Task {id: $taskId})
MERGE (e)-[:ASSIGNED_TO_TASK]->(t);";

        await session.ExecuteWriteAsync(async tx =>
        {
            foreach (var te in rows)
            {
                await tx.RunAsync(cypher, new
                {
                    employeeId = te.EmployeeId,
                    taskId = te.TaskId
                });
            }
        });

        Console.WriteLine($"  → {rows.Count} task-employee relationships migrated.");
    }*/
}