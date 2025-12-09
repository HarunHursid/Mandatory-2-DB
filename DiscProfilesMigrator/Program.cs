using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

// TODO: ret namespace + context-namespace så de matcher dit projekt
using DiscProfilesApi.Models; // <-- her: brug det namespace hvor DiscProfilesContext og entities ligger

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Starting SQL → MongoDB migration...");

        // 1) Load .env
        Env.Load();

        // SQL: brug enten SQL_CONNECTION_STRING eller CONNECTION_STRING
        var sqlConnectionString =
            Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING")
            ?? Environment.GetEnvironmentVariable("CONNECTION_STRING");

        // MONGO: prøv Atlas først, ellers lokal som fallback
        var mongoConnectionString =
            Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING_ATLAS")
            ?? Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING_LOCAL")
            ?? Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");

        // DB-navn: ny variabel MONGO_DB, fallback til gammel MONGO_DATABASE_NAME
        var mongoDatabaseName =
            Environment.GetEnvironmentVariable("MONGO_DB")
            ?? Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME");

        // simple checks
        if (string.IsNullOrWhiteSpace(sqlConnectionString))
        {
            Console.WriteLine("check sql connection string (SQL_CONNECTION_STRING / CONNECTION_STRING)");
            return;
        }
        if (string.IsNullOrWhiteSpace(mongoConnectionString))
        {
            Console.WriteLine("check mongo connection string (MONGO_CONNECTION_STRING_ATLAS / LOCAL)");
            return;
        }
        if (string.IsNullOrWhiteSpace(mongoDatabaseName))
        {
            Console.WriteLine("check mongo database name (MONGO_DB / MONGO_DATABASE_NAME)");
            return;
        }

        // 2) Setup EF Core DbContext
        var optionsBuilder = new DbContextOptionsBuilder<DiscProfilesContext>();
        optionsBuilder.UseSqlServer(sqlConnectionString);

        await using var sqlContext = new DiscProfilesContext(optionsBuilder.Options);

        // 3) Setup MongoDB
        var mongoClient = new MongoClient(mongoConnectionString);
        var mongoDb = mongoClient.GetDatabase(mongoDatabaseName);

        // 4) Kald migrations for alle tabeller
        await MigrateCompaniesAsync(sqlContext, mongoDb);
        await MigrateDepartmentsAsync(sqlContext, mongoDb);
        await MigrateDiscProfilesAsync(sqlContext, mongoDb);
        await MigrateSocialEventsAsync(sqlContext, mongoDb);
        await MigratePersonsAsync(sqlContext, mongoDb);
        await MigratePositionsAsync(sqlContext, mongoDb);
        await MigrateEducationsAsync(sqlContext, mongoDb);
        await MigrateEmployeesAsync(sqlContext, mongoDb);
        await MigrateProjectsAsync(sqlContext, mongoDb);
        await MigrateTasksAsync(sqlContext, mongoDb);
        await MigrateDailyTaskLogsAsync(sqlContext, mongoDb);
        await MigrateTaskEvaluationsAsync(sqlContext, mongoDb);
        await MigrateStressMeasuresAsync(sqlContext, mongoDb);
        await MigrateProjectsDiscProfilesAsync(sqlContext, mongoDb);
        await MigrateAppUsersAsync(sqlContext, mongoDb);
        Console.WriteLine("✅ Migration completed.");
    }

    // ------------- COMPANIES -------------

    private static async Task MigrateCompaniesAsync(DiscProfilesContext sql, IMongoDatabase mongo)
    {
        Console.WriteLine("Migrating companies...");

        var rows = await sql.companies.ToListAsync();
        var docs = rows.ConvertAll(c => new CompanyDocument
        {
            Id = c.id,
            Name = c.name,
            Location = c.location,
            BusinessField = c.business_field
        });

        var col = mongo.GetCollection<CompanyDocument>("companies");
        await col.DeleteManyAsync(_ => true);
        if (docs.Count > 0)
            await col.InsertManyAsync(docs);

        Console.WriteLine($"  → {docs.Count} companies migrated.");
    }

    // ------------- DEPARTMENTS -------------

    private static async Task MigrateDepartmentsAsync(DiscProfilesContext sql, IMongoDatabase mongo)
    {
        Console.WriteLine("Migrating departments...");

        var rows = await sql.departments.ToListAsync();
        var docs = rows.ConvertAll(d => new DepartmentDocument
        {
            Id = d.id,
            Name = d.name,
            CompanyId = d.company_id
        });

        var col = mongo.GetCollection<DepartmentDocument>("departments");
        await col.DeleteManyAsync(_ => true);
        if (docs.Count > 0)
            await col.InsertManyAsync(docs);

        Console.WriteLine($"  → {docs.Count} departments migrated.");
    }

    // ------------- DISC PROFILES -------------

    private static async Task MigrateDiscProfilesAsync(DiscProfilesContext sql, IMongoDatabase mongo)
    {
        Console.WriteLine("Migrating disc_profiles...");

        var rows = await sql.disc_profiles.ToListAsync();
        var docs = rows.ConvertAll(p => new DiscProfileDocument
        {
            Id = p.id,
            Name = p.name,
            Color = p.color,
            Description = p.description
        });

        var col = mongo.GetCollection<DiscProfileDocument>("disc_profiles");
        await col.DeleteManyAsync(_ => true);
        if (docs.Count > 0)
            await col.InsertManyAsync(docs);

        Console.WriteLine($"  → {docs.Count} disc_profiles migrated.");
    }

    // ------------- SOCIAL EVENTS -------------

    private static async Task MigrateSocialEventsAsync(DiscProfilesContext sql, IMongoDatabase mongo)
    {
        Console.WriteLine("Migrating social_events...");

        var rows = await sql.social_events.ToListAsync();
        var docs = rows.ConvertAll(s => new SocialEventDocument
        {
            Id = s.id,
            Name = s.name,
            DiscProfileId = s.disc_profile_id,
            Description = s.description,
            CompanyId = s.company_id
        });

        var col = mongo.GetCollection<SocialEventDocument>("social_events");
        await col.DeleteManyAsync(_ => true);
        if (docs.Count > 0)
            await col.InsertManyAsync(docs);

        Console.WriteLine($"  → {docs.Count} social_events migrated.");
    }

    // ------------- PERSONS -------------

    private static async Task MigratePersonsAsync(DiscProfilesContext sql, IMongoDatabase mongo)
    {
        Console.WriteLine("Migrating persons...");

        var rows = await sql.persons.ToListAsync();
        var docs = rows.ConvertAll(p => new PersonDocument
        {
            Id = p.id,
            PrivateEmail = p.private_email,
            PrivatePhone = p.private_phone,
            Cpr = p.cpr,
            FirstName = p.first_name,
            LastName = p.last_name,
            Experience = p.experience,
            EducationId = p.EducationID
        });

        var col = mongo.GetCollection<PersonDocument>("persons");
        await col.DeleteManyAsync(_ => true);
        if (docs.Count > 0)
            await col.InsertManyAsync(docs);

        Console.WriteLine($"  → {docs.Count} persons migrated.");
    }

    // ------------- POSITIONS -------------

    private static async Task MigratePositionsAsync(DiscProfilesContext sql, IMongoDatabase mongo)
    {
        Console.WriteLine("Migrating positions...");

        var rows = await sql.positions.ToListAsync();
        var docs = rows.ConvertAll(p => new PositionDocument
        {
            Id = p.id,
            Name = p.name
        });

        var col = mongo.GetCollection<PositionDocument>("positions");
        await col.DeleteManyAsync(_ => true);
        if (docs.Count > 0)
            await col.InsertManyAsync(docs);

        Console.WriteLine($"  → {docs.Count} positions migrated.");
    }

    // ------------- EDUCATIONS -------------

    private static async Task MigrateEducationsAsync(DiscProfilesContext sql, IMongoDatabase mongo)
    {
        Console.WriteLine("Migrating educations...");

        var rows = await sql.educations.ToListAsync();
        var docs = rows.ConvertAll(e => new EducationDocument
        {
            Id = e.id,
            Name = e.name,
            Type = e.type,
            Grade = e.grade
        });

        var col = mongo.GetCollection<EducationDocument>("educations");
        await col.DeleteManyAsync(_ => true);
        if (docs.Count > 0)
            await col.InsertManyAsync(docs);

        Console.WriteLine($"  → {docs.Count} educations migrated.");
    }

    // ------------- EMPLOYEES -------------

    private static async Task MigrateEmployeesAsync(DiscProfilesContext sql, IMongoDatabase mongo)
    {
        Console.WriteLine("Migrating employees...");

        var rows = await sql.employees.ToListAsync();

        var docs = rows.ConvertAll(e => new EmployeeDocument
        {

            Id = e.id,
            Email = e.email,
            Phone = e.phone,
            CompanyId = e.company_id,
            PersonId = e.person_id,
            DepartmentId = e.department_id,
            PositionId = e.position_id,
            DiscProfileId = e.disc_profile_id,
            CreatedAt = e.CreatedAt,
            IsActive = e.IsActive,
            LastLogin = e.LastLogin
        });

        var col = mongo.GetCollection<EmployeeDocument>("employees");
        await col.DeleteManyAsync(_ => true);
        if (docs.Count > 0)
            await col.InsertManyAsync(docs);

        Console.WriteLine($"  → {docs.Count} employees migrated.");
    }

    // ------------- PROJECTS -------------

    private static async Task MigrateProjectsAsync(DiscProfilesContext sql, IMongoDatabase mongo)
    {
        Console.WriteLine("Migrating projects...");

        var rows = await sql.projects.ToListAsync();
        var docs = rows.ConvertAll(p => new ProjectDocument
        {
            Id = p.id,
            Name = p.name,
            Description = p.description,
            Deadline = p.deadline,
            Completed = p.completed,
            NumberOfEmployees = p.number_of_employees
        });

        var col = mongo.GetCollection<ProjectDocument>("projects");
        await col.DeleteManyAsync(_ => true);
        if (docs.Count > 0)
            await col.InsertManyAsync(docs);

        Console.WriteLine($"  → {docs.Count} projects migrated.");
    }

    // ------------- TASKS -------------

    private static async Task MigrateTasksAsync(DiscProfilesContext sql, IMongoDatabase mongo)
    {
        Console.WriteLine("Migrating tasks...");

        var rows = await sql.tasks.ToListAsync();
        var docs = rows.ConvertAll(t => new TaskDocument
        {
            Id = t.id,
            Name = t.name,
            Completed = t.completed,
            TimeOfCompletion = t.time_of_completion,
            ProjectId = t.project_id
        });

        var col = mongo.GetCollection<TaskDocument>("tasks");
        await col.DeleteManyAsync(_ => true);
        if (docs.Count > 0)
            await col.InsertManyAsync(docs);

        Console.WriteLine($"  → {docs.Count} tasks migrated.");
    }

    // ------------- DAILY TASK LOGS -------------

    private static async Task MigrateDailyTaskLogsAsync(DiscProfilesContext sql, IMongoDatabase mongo)
    {
        Console.WriteLine("Migrating daily_task_logs...");

        var rows = await sql.daily_task_logs.ToListAsync();
        var docs = rows.ConvertAll(l => new DailyTaskLogDocument
        {
            Id = l.id,
            TimeToComplete = l.time_to_finish,
            TaskId = l.task_id
        });

        var col = mongo.GetCollection<DailyTaskLogDocument>("daily_task_logs");
        await col.DeleteManyAsync(_ => true);
        if (docs.Count > 0)
            await col.InsertManyAsync(docs);

        Console.WriteLine($"  → {docs.Count} daily_task_logs migrated.");
    }

    // ------------- TASK EVALUATIONS -------------

    private static async Task MigrateTaskEvaluationsAsync(DiscProfilesContext sql, IMongoDatabase mongo)
    {
        Console.WriteLine("Migrating task_evaluations...");

        var rows = await sql.task_evaluations.ToListAsync();
        var docs = rows.ConvertAll(e => new TaskEvaluationDocument
        {
            Id = e.id,
            Description = e.description,
            DifficultyRange = e.difficulty_range,
            TaskId = e.task_id
        });

        var col = mongo.GetCollection<TaskEvaluationDocument>("task_evaluations");
        await col.DeleteManyAsync(_ => true);
        if (docs.Count > 0)
            await col.InsertManyAsync(docs);

        Console.WriteLine($"  → {docs.Count} task_evaluations migrated.");
    }

    // ------------- STRESS MEASURES -------------

    private static async Task MigrateStressMeasuresAsync(DiscProfilesContext sql, IMongoDatabase mongo)
    {
        Console.WriteLine("Migrating stress_measures...");

        var rows = await sql.stress_measures.ToListAsync();
        var docs = rows.ConvertAll(s => new StressMeasureDocument
        {
            Id = s.id,
            Description = s.description,
            Measure = s.measure,
            EmployeeId = s.employee_id,
            TaskId = s.task_id
        });

        var col = mongo.GetCollection<StressMeasureDocument>("stress_measures");
        await col.DeleteManyAsync(_ => true);
        if (docs.Count > 0)
            await col.InsertManyAsync(docs);

        Console.WriteLine($"  → {docs.Count} stress_measures migrated.");
    }


    // ------------- PROJECTS_DISC_PROFILES -------------

    private static async Task MigrateProjectsDiscProfilesAsync(DiscProfilesContext sql, IMongoDatabase mongo)
    {
        Console.WriteLine("Migrating projects_disc_profiles...");

        var rows = await sql.projects_disc_profiles.ToListAsync();
        var docs = rows.ConvertAll(p => new ProjectsDiscProfilesDocument
        {
            Id = p.id,
            ProjectId = p.project_id,
            DiscProfileId = p.disc_profile_id
        });

        var col = mongo.GetCollection<ProjectsDiscProfilesDocument>("projects_disc_profiles");
        await col.DeleteManyAsync(_ => true);
        if (docs.Count > 0)
            await col.InsertManyAsync(docs);

        Console.WriteLine($"  → {docs.Count} projects_disc_profiles migrated.");
    }

    // ------------- APP USERS ------------- 

    private static async Task MigrateAppUsersAsync(DiscProfilesContext sql, IMongoDatabase mongo)
    {
        Console.WriteLine("Migrating app_users...");

        var rows = await sql.AppUsers.ToListAsync();
        var docs = rows.ConvertAll(u => new AppUserDocument
        {
            Id = u.Id,
            Email = u.Email,
            PasswordHash = u.PasswordHash,
            Role = u.Role,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            LastLogin = u.LastLogin,
            EmployeeId = u.EmployeeId
        });

        var col = mongo.GetCollection<AppUserDocument>("app_users");
        await col.DeleteManyAsync(_ => true);  // ryd collection før insert
        if (docs.Count > 0)
            await col.InsertManyAsync(docs);

        Console.WriteLine($"  → {docs.Count} app_users migrated.");
    }
}

// ----------------- DOCUMENT-CLASSES TIL MONGO -----------------

public class CompanyDocument
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("location")]
    public string? Location { get; set; }

    [BsonElement("business_field")]
    public string? BusinessField { get; set; }
}

public class DepartmentDocument
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("company_id")]
    public int CompanyId { get; set; }
}

public class DiscProfileDocument
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("color")]
    public string? Color { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }
}

public class SocialEventDocument
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("disc_profile_id")]
    public int? DiscProfileId { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("company_id")]
    public int? CompanyId { get; set; }
}

public class PersonDocument
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("private_email")]
    public string? PrivateEmail { get; set; }

    [BsonElement("private_phone")]
    public string? PrivatePhone { get; set; }

    [BsonElement("cpr")]
    public string Cpr { get; set; } = string.Empty;

    [BsonElement("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [BsonElement("last_name")]
    public string LastName { get; set; } = string.Empty;

    [BsonElement("experience")]
    public int? Experience { get; set; }

    [BsonElement("education_id")]
    public int? EducationId { get; set; }
}

public class PositionDocument
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;
}

public class EducationDocument
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("type")]
    public string? Type { get; set; }

    [BsonElement("grade")]
    public int? Grade { get; set; }
}

public class EmployeeDocument
{
        [BsonId]
        public int Id { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("phone")]
        public string? Phone { get; set; }

        [BsonElement("company_id")]
        public int CompanyId { get; set; }

        [BsonElement("person_id")]
        public int? PersonId { get; set; }

        [BsonElement("department_id")]
        public int? DepartmentId { get; set; }

        [BsonElement("position_id")]
        public int? PositionId { get; set; }

        [BsonElement("disc_profile_id")]
        public int? DiscProfileId { get; set; }

        [BsonElement("created_at")]
        public DateTime? CreatedAt { get; set; }

        [BsonElement("is_active")]
        public bool? IsActive { get; set; }

        [BsonElement("last_login")]
        public DateTime? LastLogin { get; set; }
    
}

public class ProjectDocument
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("deadline")]
    public string? Deadline { get; set; }

    [BsonElement("completed")]
    public string? Completed { get; set; }

    [BsonElement("number_of_employees")]
    public int? NumberOfEmployees { get; set; }
}

public class TaskDocument
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("completed")]
    public bool Completed { get; set; }

    [BsonElement("time_of_completion")]
    public DateTime? TimeOfCompletion { get; set; }

    [BsonElement("project_id")]
    public int? ProjectId { get; set; }
}

public class DailyTaskLogDocument
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("time_to_complete")]
    public string? TimeToComplete { get; set; }

    [BsonElement("task_id")]
    public int? TaskId { get; set; }
}

public class TaskEvaluationDocument
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("difficulty_range")]
    public int? DifficultyRange { get; set; }

    [BsonElement("task_id")]
    public int? TaskId { get; set; }
}

public class StressMeasureDocument
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("measure")]
    public int? Measure { get; set; }

    [BsonElement("employee_id")]
    public int? EmployeeId { get; set; }

    [BsonElement("task_id")]
    public int? TaskId { get; set; }
}



public class ProjectsDiscProfilesDocument
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("project_id")]
    public int ProjectId { get; set; }

    [BsonElement("disc_profile_id")]
    public int DiscProfileId { get; set; }
}

public class AppUserDocument
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("role")]
    public string Role { get; set; } = "user";

    [BsonElement("is_active")]
    public bool IsActive { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("last_login")]
    public DateTime? LastLogin { get; set; }

    [BsonElement("employee_id")]
    public int? EmployeeId { get; set; }
}
