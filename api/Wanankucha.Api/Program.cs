using System.IO.Compression;
using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Wanankucha.Api.Application;
using Wanankucha.Api.Infrastructure;
using Wanankucha.Api.Jobs;
using Wanankucha.Api.Middlewares;
using Wanankucha.Api.Persistence;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting Wanankucha API...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddApplicationServices();
    builder.Services.AddPersistenceServices(builder.Configuration);
    builder.Services.AddInfrastructureServices(builder.Configuration);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // ========================
    // Health Checks
    // ========================
    var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
    builder.Services.AddHealthChecks()
        .AddNpgSql(connectionString!, name: "postgresql", tags: ["db", "postgresql"]);

    // ========================
    // Distributed Caching (In-Memory)
    // ========================
    builder.Services.AddDistributedMemoryCache();

    // ========================
    // Response Compression (Brotli + Gzip)
    // ========================
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[] { "application/json", "text/json" });
    });
    builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
        options.Level = CompressionLevel.Fastest);
    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
        options.Level = CompressionLevel.SmallestSize);

    // ========================
    // Response Caching (Output Cache)
    // ========================
    builder.Services.AddOutputCache(options =>
    {
        options.AddBasePolicy(b => b.Expire(TimeSpan.FromMinutes(1)));
        options.AddPolicy("Users", b => b.Expire(TimeSpan.FromMinutes(5)).Tag("users"));
    });

    // ========================
    // Background Jobs (Hangfire)
    // ========================
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

    builder.Services.AddHangfireServer();
    builder.Services.AddScoped<CleanupExpiredTokensJob>();

    // ========================
    // Rate Limiting
    // ========================
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        options.AddFixedWindowLimiter("auth", config =>
        {
            config.PermitLimit = 5;
            config.Window = TimeSpan.FromMinutes(1);
            config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            config.QueueLimit = 0;
        });

        options.OnRejected = async (context, cancellationToken) =>
        {
            Log.Warning("Rate limit exceeded for {Path} from {IP}",
                context.HttpContext.Request.Path,
                context.HttpContext.Connection.RemoteIpAddress);

            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.HttpContext.Response.WriteAsync(
                "Too many requests. Please try again later.", cancellationToken);
        };
    });

    // ========================
    // API Versioning
    // ========================
    builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"));
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

    // CORS for Blazor web app (tightened)
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("BlazorWebApp", policy =>
        {
            policy.WithOrigins("https://localhost:5001", "http://localhost:5279")
                .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                .WithHeaders("Content-Type", "Authorization", "X-Api-Version")
                .AllowCredentials();
        });
    });

    builder.Services.AddAuthorization();

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = builder.Configuration["Token:Audience"],
                ValidIssuer = builder.Configuration["Token:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"]
                                           ?? throw new InvalidOperationException(
                                               "Token:SecurityKey configuration is required"))),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    Log.Warning("JWT authentication failed: {Message}", ctx.Exception?.Message);
                    return Task.CompletedTask;
                },
                OnMessageReceived = ctx => Task.CompletedTask
            };
        });

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Wanankucha API", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Format: 'Bearer {token}'"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                []
            }
        });
    });

    var app = builder.Build();

    // ========================
    // Startup Database Check (fail fast if DB unreachable)
    // ========================
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<Wanankucha.Api.Persistence.Contexts.AppDbContext>();
        try
        {
            Log.Information("Checking database connectivity...");
            var canConnect = await dbContext.Database.CanConnectAsync();
            if (!canConnect)
            {
                throw new Exception("Cannot connect to PostgreSQL database. Please ensure the database is running.");
            }
            Log.Information("Database connection verified successfully");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Database connection failed. Please ensure PostgreSQL is running.");
            throw;
        }
    }

    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    // Response Compression middleware (before static files)
    app.UseResponseCompression();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wanankucha API v1"));
    }
    else
    {
        // HSTS (Strict Transport Security) - only in production
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseCors("BlazorWebApp");

    // Rate limiting middleware
    app.UseRateLimiter();

    // Output cache middleware
    app.UseOutputCache();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Health check endpoint
    app.MapHealthChecks("/health");

    // Hangfire dashboard (secured - local access only in production)
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

    Log.Information("Wanankucha API started successfully");
    Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);
    Log.Information("Swagger UI: https://localhost:7230/swagger");
    Log.Information("Health Check: https://localhost:7230/health");
    Log.Information("Hangfire Dashboard: https://localhost:7230/hangfire");

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