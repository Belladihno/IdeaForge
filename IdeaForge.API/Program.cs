using IdeaForge.API.Common;
using IdeaForge.API.Middleware;
using IdeaForge.Application;
using IdeaForge.Infrastructure;
using IdeaForge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Hosts like Render assign the HTTP port via PORT. Only honor it when set,
// so local launch profiles keep working untouched.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://+:{port}");
}

builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration)
        .WriteTo.Console());

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.UseInlineDefinitionsForEnums());

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddTransient(
    typeof(MediatR.IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));

// Comma- or semicolon-separated allowed frontend origins. Local default is the
// Vite dev server; set FRONTEND_URLS to the deployed frontend URL in production.
var frontendUrls = (Environment.GetEnvironmentVariable("FRONTEND_URLS") ?? "http://localhost:5173")
    .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(frontendUrls)
            .AllowAnyHeader()
            .AllowAnyMethod()));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Only redirect to HTTPS when explicitly enabled. Behind a TLS-terminating
// proxy (Render) this would loop — enable it only when the app terminates TLS itself.
if (Environment.GetEnvironmentVariable("ENABLE_HTTPS_REDIRECT") == "true")
{
    app.UseHttpsRedirection();
}

app.UseCors("Frontend");
app.MapControllers();

// Health endpoint for host health checks (e.g. Render healthCheckPath).
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

// PaaS hosts have no EF CLI, so apply pending migrations at startup.
// Retries cover cold database computes (e.g. Neon free tier waking up).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    const int maxAttempts = 5;
    for (var attempt = 1; ; attempt++)
    {
        try
        {
            await db.Database.MigrateAsync();
            app.Logger.LogInformation("Database migrations applied.");
            break;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            app.Logger.LogWarning(ex, "Migration attempt {Attempt} failed, retrying...", attempt);
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}

app.Run();
