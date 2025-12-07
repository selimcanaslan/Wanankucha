using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wanankucha.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Wanankucha.Application.Abstractions;
using Wanankucha.Domain.Repositories;
using Wanankucha.Persistence.Repositories;
using Wanankucha.Persistence.Services;

namespace Wanankucha.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
        
        services.AddScoped<IUserService, UserService>();
    }
}