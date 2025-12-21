using DotNetEnv;
using Serilog;
using Wanankucha.Api.Application;
using Wanankucha.Api.Extensions;
using Wanankucha.Api.Infrastructure;
using Wanankucha.Api.Jobs;
using Wanankucha.Api.Persistence;

// Load .env file (if exists) before anything else
Env.TraversePath().Load();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting Wanankucha API...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ========================
    // Serilog
    // ========================
    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    // ========================
    // Layer Services (Clean Architecture)
    // ========================
    builder.Services.AddApplicationServices();
    builder.Services.AddPersistenceServices(builder.Configuration);
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // ========================
    // API Services
    // ========================
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // Infrastructure
    builder.Services.AddHealthChecksServices(builder.Configuration);
    builder.Services.AddCachingServices();
    builder.Services.AddCompressionServices();
    builder.Services.AddHangfireServices(builder.Configuration);
    builder.Services.AddScoped<CleanupExpiredTokensJob>();

    // Security
    builder.Services.AddRateLimitingServices();
    builder.Services.AddCorsServices(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);

    // API Features
    builder.Services.AddApiVersioningServices();
    builder.Services.AddSwaggerServices();

    // ========================
    // Build & Configure Pipeline
    // ========================
    var app = builder.Build();

    // Startup database check (fail fast)
    await app.EnsureDatabaseConnectedAsync();

    // Middleware pipeline
    app.UseCommonMiddleware();

    // Endpoints
    app.MapApiEndpoints();
    app.UseHangfireDashboardAndJobs();

    // Log startup info
    app.LogStartupInfo();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}