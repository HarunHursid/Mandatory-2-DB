using DiscProfilesApi.DTOs;
using DiscProfilesApi.Interfaces;
using DiscProfilesApi.Mappings;
using DiscProfilesApi.Models;
using DiscProfilesApi.MongoDocuments;
using DiscProfilesApi.Repositories;
using DiscProfilesApi.Services;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Text;

using Neo4j.Driver;
using DiscProfilesApi.Services.GraphServices;

var builder = WebApplication.CreateBuilder(args);

// -------- ENV VARS --------
Env.Load();

// SQL connection fra .env
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING_ATLAS");
var mongoDatabaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME_ATLAS");

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDatabaseName);
});

// Generic repo for alle dokumenttyper
builder.Services.AddScoped(typeof(IGenericMongoRepository<>), typeof(GenericMongoRepository<>));

builder.Services.AddScoped<
    IGenericMongoService<CompanyDocument, CompanyDTO>,
    GenericMongoService<CompanyDocument, CompanyDTO>>();

builder.Services.AddScoped<
    IGenericMongoService<AppUserDocument, AppUserDTO>,
    GenericMongoService<AppUserDocument, AppUserDTO>>();

builder.Services.AddScoped<
    IGenericMongoService<DiscProfileDocument, DiscProfileDTO>,
    GenericMongoService<DiscProfileDocument, DiscProfileDTO>>();

builder.Services.AddScoped<
    IGenericMongoService<EmployeeDocument, EmployeeDTO>,
    GenericMongoService<EmployeeDocument, EmployeeDTO>>();

builder.Services.AddScoped<
    IGenericMongoService<ProjectDocument, ProjectDTO>,
    GenericMongoService<ProjectDocument, ProjectDTO>>();

builder.Services.AddScoped<
    IGenericMongoService<DepartmentDocument, DepartmentDTO>,
    GenericMongoService<DepartmentDocument, DepartmentDTO>>();

builder.Services.AddScoped<
    IGenericMongoService<EducationDocument, EducationDTO>,
    GenericMongoService<EducationDocument, EducationDTO>>();

builder.Services.AddScoped<
    IGenericMongoService<PersonDocument, PersonDTO>,
    GenericMongoService<PersonDocument, PersonDTO>>();

builder.Services.AddScoped<
    IGenericMongoService<PositionDocument, PositionDTO>,
    GenericMongoService<PositionDocument, PositionDTO>>();

builder.Services.AddScoped<
    IGenericMongoService<TaskDocument, TaskDTO>,
    GenericMongoService<TaskDocument, TaskDTO>>();

builder.Services.AddScoped<
    IGenericMongoService<SocialEventDocument, SocialEventDTO>,
    GenericMongoService<SocialEventDocument, SocialEventDTO>>();

builder.Services.AddScoped<
    IGenericMongoService<DailyTaskLogDocument, DailyTaskLogDTO>,
    GenericMongoService<DailyTaskLogDocument, DailyTaskLogDTO>>();

builder.Services.AddScoped<
    IGenericMongoService<TaskEvaluationDocument, TaskEvaluationDTO>,
    GenericMongoService<TaskEvaluationDocument, TaskEvaluationDTO>>();

builder.Services.AddScoped<
    IGenericMongoService<StressMeasureDocument, StressMeasureDTO>,
    GenericMongoService<StressMeasureDocument, StressMeasureDTO>>();

builder.Services.AddScoped<
    IGenericMongoService<ProjectsDiscProfilesDocument, ProjectsDiscProfileDTO>,
    GenericMongoService<ProjectsDiscProfilesDocument, ProjectsDiscProfileDTO>>();


// NEO4J connection fra .env
var neo4jUri = Environment.GetEnvironmentVariable("NEO4J_URI");
var neo4jUser = Environment.GetEnvironmentVariable("NEO4J_USER");
var neo4jPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD");
// database-navn bruger du i din GraphEmployeeService
// NEO4J_DATABASE læses derinde via Environment.GetEnvironmentVariable("NEO4J_DATABASE")

// -------- SERVICES --------

// SQL DbContext
builder.Services.AddDbContext<DiscProfilesContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DiscProfilesApi",
        Version = "v1"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Put **Bearer {token}** in the box below",

        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

// Authentication
var jwtConfig = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtConfig["Key"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization
builder.Services.AddAuthorization();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Generic DI
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IEmployee_PersonDomainInterface, Employee_PersonDomainService>();

// -------- NEO4J DRIVER DI --------
builder.Services.AddSingleton<IDriver>(_ =>
    GraphDatabase.Driver(neo4jUri, AuthTokens.Basic(neo4jUser, neo4jPassword))
);

// Graph service til Neo4j
builder.Services.AddScoped<GraphEmployeeService>();
builder.Services.AddScoped<GraphCompanyService>();
builder.Services.AddScoped<GraphProjectService>();
builder.Services.AddScoped<GraphDiscProfileService>();
builder.Services.AddScoped<GraphDepartmentService>();
builder.Services.AddScoped<GraphTaskService>();
builder.Services.AddScoped<GraphPersonService>();
builder.Services.AddScoped<GraphEducationService>();
builder.Services.AddScoped<GraphPositionService>();
builder.Services.AddScoped<GraphSocialEventService>();
builder.Services.AddScoped<GraphStressMeasureService>();
builder.Services.AddScoped<GraphDailyTaskLogService>();
builder.Services.AddScoped<GraphTaskEvaluationService>();
builder.Services.AddScoped<GraphProjectsDiscProfilesService>();

var app = builder.Build();

// -------- PIPELINE --------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DiscProfiles API v1");
    c.RoutePrefix = "swagger"; // så swagger ligger på /swagger
});

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
