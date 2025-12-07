using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wanankucha.Domain.Entities.Identity;
using Wanankucha.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Wanankucha.Domain.Repositories;
using Wanankucha.Persistence.Repositories;

namespace Wanankucha.Persistence;

public static class ServiceRegistration
{
    // IServiceCollection ve IConfiguration extension metod parametreleri
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext'i ekliyoruz. Bağlantı cümlesini (ConnectionString) Configuration'dan alacağız.
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        // Identity servislerini ekliyoruz.
        services.AddIdentity<AppUser, AppRole>(options =>
            {
                // Şifre kuralları (isteğe bağlı gevşetilebilir)
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<AppDbContext>();

        // Generic Repository Kayıtları
        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
    }
}