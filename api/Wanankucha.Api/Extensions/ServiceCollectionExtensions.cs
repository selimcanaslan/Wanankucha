using System.IO.Compression;
using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Wanankucha.Api.Extensions;

/// <summary>
/// Service registration extensions for cleaner Program.cs
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Health Checks for PostgreSQL
    /// </summary>
    public static IServiceCollection AddHealthChecksServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("ConnectionStrings:PostgreSQL is required");
            
        services.AddHealthChecks()
            .AddNpgSql(connectionString, name: "postgresql", tags: ["db", "postgresql"]);
            
        return services;
    }

    /// <summary>
    /// Add Caching (Distributed Memory Cache + Output Cache)
    /// </summary>
    public static IServiceCollection AddCachingServices(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        
        services.AddOutputCache(options =>
        {
            options.AddBasePolicy(b => b.Expire(TimeSpan.FromMinutes(1)));
            options.AddPolicy("Users", b => b.Expire(TimeSpan.FromMinutes(5)).Tag("users"));
        });
        
        return services;
    }

    /// <summary>
    /// Add Response Compression (Brotli + Gzip)
    /// </summary>
    public static IServiceCollection AddCompressionServices(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[] { "application/json", "text/json" });
        });
        
        services.Configure<BrotliCompressionProviderOptions>(options =>
            options.Level = CompressionLevel.Fastest);
        services.Configure<GzipCompressionProviderOptions>(options =>
            options.Level = CompressionLevel.SmallestSize);
            
        return services;
    }

    /// <summary>
    /// Add Hangfire for background job processing
    /// </summary>
    public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("ConnectionStrings:PostgreSQL is required");
            
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

        services.AddHangfireServer();
        
        return services;
    }

    /// <summary>
    /// Add Rate Limiting with auth policy
    /// </summary>
    public static IServiceCollection AddRateLimitingServices(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
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
                Serilog.Log.Warning("Rate limit exceeded for {Path} from {IP}",
                    context.HttpContext.Request.Path,
                    context.HttpContext.Connection.RemoteIpAddress);

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsync(
                    "Too many requests. Please try again later.", cancellationToken);
            };
        });
        
        return services;
    }

    /// <summary>
    /// Add API Versioning with URL segment and header readers
    /// </summary>
    public static IServiceCollection AddApiVersioningServices(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
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
        
        return services;
    }

    /// <summary>
    /// Add CORS policy for Blazor web app
    /// </summary>
    public static IServiceCollection AddCorsServices(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("BlazorWebApp", policy =>
            {
                policy.WithOrigins("https://localhost:5001", "http://localhost:5279")
                    .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                    .WithHeaders("Content-Type", "Authorization", "X-Api-Version")
                    .AllowCredentials();
            });
        });
        
        return services;
    }

    /// <summary>
    /// Add JWT Authentication
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization();

        services.AddAuthentication(options =>
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
                ValidAudience = configuration["Token:Audience"],
                ValidIssuer = configuration["Token:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Token:SecurityKey"]
                        ?? throw new InvalidOperationException("Token:SecurityKey configuration is required"))),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    Serilog.Log.Warning("JWT authentication failed: {Message}", ctx.Exception?.Message);
                    return Task.CompletedTask;
                },
                OnMessageReceived = ctx => Task.CompletedTask
            };
        });
        
        return services;
    }

    /// <summary>
    /// Add Swagger/OpenAPI with JWT security
    /// </summary>
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
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
        
        return services;
    }
}
