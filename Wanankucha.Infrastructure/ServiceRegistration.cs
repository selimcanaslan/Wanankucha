using Microsoft.Extensions.DependencyInjection;
using Wanankucha.Application.Abstractions;
using Wanankucha.Infrastructure.Services.Encryption;
using Wanankucha.Infrastructure.Services.Token;

namespace Wanankucha.Infrastructure;

public static class ServiceRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
    }
}