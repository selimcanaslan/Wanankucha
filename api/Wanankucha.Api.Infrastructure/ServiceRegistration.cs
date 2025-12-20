using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Infrastructure.Options;
using Wanankucha.Api.Infrastructure.Services.Email;
using Wanankucha.Api.Infrastructure.Services.Encryption;
using Wanankucha.Api.Infrastructure.Services.Token;

namespace Wanankucha.Api.Infrastructure;

public static class ServiceRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IEmailService, SmtpEmailService>();
    }
}