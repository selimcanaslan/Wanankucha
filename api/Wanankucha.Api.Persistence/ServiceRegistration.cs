using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wanankucha.Api.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Wanankucha.Api.Domain.Repositories;
using Wanankucha.Api.Persistence.Repositories;

namespace Wanankucha.Api.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}