using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wanankucha.Api.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Wanankucha.Api.Domain.Repositories;
using Wanankucha.Api.Persistence.Interceptors;
using Wanankucha.Api.Persistence.Repositories;

namespace Wanankucha.Api.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL");

        // Register the audit interceptor as a singleton
        services.AddSingleton<AuditSaveChangesInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(60);
            })
            .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
