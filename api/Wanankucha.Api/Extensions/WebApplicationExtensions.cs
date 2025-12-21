using Hangfire;
using Hangfire.Dashboard;
using Microsoft.EntityFrameworkCore;
using Wanankucha.Api.Jobs;
using Wanankucha.Api.Middlewares;
using Wanankucha.Api.Persistence.Contexts;

namespace Wanankucha.Api.Extensions;

/// <summary>
/// WebApplication middleware extensions for cleaner Program.cs
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Verify database connectivity at startup (fail-fast)
    /// </summary>
    public static async Task EnsureDatabaseConnectedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var config = app.Configuration;
        
        try
        {
            // Debug: Check direct environment variable
            var envConnString = Environment.GetEnvironmentVariable("ConnectionStrings__PostgreSQL");
            Serilog.Log.Information("Direct ENV ConnectionStrings__PostgreSQL: {Exists}, Length: {Length}", 
                !string.IsNullOrEmpty(envConnString), envConnString?.Length ?? 0);
            
            var connString = config.GetConnectionString("PostgreSQL");
            Serilog.Log.Information("Checking database connectivity...");
            Serilog.Log.Information("Config connection string length: {Length}", connString?.Length ?? 0);
            Serilog.Log.Information("Connection string starts with: {Start}", 
                connString?.Substring(0, Math.Min(50, connString?.Length ?? 0)) ?? "null");
            
            // If env var exists but config doesn't have it, use env var directly
            if (!string.IsNullOrEmpty(envConnString) && (connString?.Contains("localhost") ?? true))
            {
                Serilog.Log.Warning("Using direct environment variable instead of config");
                connString = envConnString;
            }
            
            // Try to execute a simple query to get the actual error
            await dbContext.Database.ExecuteSqlRawAsync("SELECT 1");
            
            Serilog.Log.Information("Database connection verified successfully");
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Database connection failed: {Message}", ex.Message);
            Serilog.Log.Error("Inner exception: {Inner}", ex.InnerException?.Message ?? "none");
            throw new Exception($"Cannot connect to PostgreSQL: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Configure common middleware pipeline
    /// </summary>
    public static WebApplication UseCommonMiddleware(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        app.UseResponseCompression();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wanankucha API v1"));
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseCors("BlazorWebApp");
        app.UseRateLimiter();
        app.UseOutputCache();
        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }

    /// <summary>
    /// Map API endpoints (controllers, health checks, root)
    /// </summary>
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapControllers();
        
        // Root endpoint - redirect to Swagger or show API info
        app.MapGet("/", (IWebHostEnvironment env) =>
        {
            if (env.IsDevelopment())
            {
                return Results.Redirect("/swagger");
            }

            return Results.Ok(new
            {
                Name = "Wanankucha API",
                Version = "1.0",
                Status = "Running",
                Documentation = "/swagger",
                HealthCheck = "/health",
                Dashboard = "/hangfire"
            });
        }).ExcludeFromDescription();

        app.MapHealthChecks("/health");
        
        return app;
    }

    /// <summary>
    /// Configure Hangfire dashboard and recurring jobs
    /// </summary>
    public static WebApplication UseHangfireDashboardAndJobs(this WebApplication app)
    {
        app.MapHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = app.Environment.IsDevelopment()
                ? Array.Empty<IDashboardAuthorizationFilter>()
                : new[] { new LocalRequestsOnlyAuthorizationFilter() }
        });

        // Register recurring jobs
        RecurringJob.AddOrUpdate<CleanupExpiredTokensJob>(
            "cleanup-expired-tokens",
            job => job.ExecuteAsync(),
            Cron.Hourly);
            
        return app;
    }

    /// <summary>
    /// Log startup information
    /// </summary>
    public static void LogStartupInfo(this WebApplication app)
    {
        Serilog.Log.Information("Wanankucha API started successfully");
        Serilog.Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);
        Serilog.Log.Information("Swagger UI: https://localhost:7230/swagger");
        Serilog.Log.Information("Health Check: https://localhost:7230/health");
        Serilog.Log.Information("Hangfire Dashboard: https://localhost:7230/hangfire");
    }
}

/// <summary>
/// Local-only authorization filter for Hangfire dashboard
/// </summary>
public class LocalRequestsOnlyAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return httpContext.Request.Host.Host is "localhost" or "127.0.0.1" or "::1";
    }
}
