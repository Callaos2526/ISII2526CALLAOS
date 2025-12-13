using AppForSEII2526.API.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddRabbitMQ(builder.Configuration.GetSection("RabbitMQ"));

// Add services to the container.
builder.Services.AddControllers()
    // show definitions of enums as strings
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Database selection by environment variable
string? connection2Database = Environment.GetEnvironmentVariable("DBConnection2Use");

// Configure DB contexts
switch (connection2Database)
{
    case "SQLite":
        DbConnection _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlite(_connection));
        break;

    case "AzureSQL":
        builder.Services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseSqlServer(Environment.GetEnvironmentVariable("AzureSQL")));
        break;

    default:
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        break;
}

// Add Identity / Authorization
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "AppForSEII2526.API",
            Version = "v1",
            Description = "This API provides services for renting and purchasing movies",
            License = new OpenApiLicense { Name = "MIT License", Url = new Uri("https://opensource.org/license/mit/") },
            Contact = new OpenApiContact { Name = "Software Engineering II Team", Email = "isii@on.uclm.es" },
        });

    options.CustomOperationIds(apiDescription =>
        apiDescription.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalDev", policy =>
    {
        policy
            .WithOrigins("https://localhost:7067", "http://localhost:7067", "https://localhost:5001", "http://localhost:5000")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowLocalDev");

// obtener logger para mensajes de inicialización
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Inicialización de la base de datos: aplicar migraciones solo si hay pendientes.
// Evita lanzar excepción si la BD ya contiene tablas (por ejemplo AspNetRoles).
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (connection2Database == "SQLite")
        {
            db.Database.EnsureCreated();
            logger.LogInformation("SQLite in-memory DB ensured created.");
        }
        else
        {
            var pending = db.Database.GetPendingMigrations().ToList();
            if (pending.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations: {Migrations}", pending.Count, string.Join(", ", pending));
                db.Database.Migrate();
            }
            else
            {
                logger.LogInformation("No pending migrations. Database is up to date.");
            }
        }

        // SeedData.Initialize(db, scope.ServiceProvider, logger); // descomentar si tienes SeedData y quieres ejecutarlo
    }
    catch (SqlException sqlEx) when (sqlEx.Number == 2714)
    {
        // 2714 = objeto ya existe: evitar crash y dar pista
        logger.LogWarning(sqlEx, "SQL error 2714 while applying migrations: objeto ya existe en la BD. " +
            "Si estás en desarrollo borra la BD local y vuelve a aplicar migraciones, o sincroniza manualmente la tabla __EFMigrationsHistory.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.DisplayOperationId());
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }