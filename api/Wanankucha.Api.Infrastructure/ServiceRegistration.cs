using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Resend;
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
        
        // Email service: Use Resend if API key starts with "re_", otherwise use SMTP
        var smtpPassword = configuration["Smtp:Password"] ?? "";
        if (smtpPassword.StartsWith("re_"))
        {
            // Configure Resend HTTP client
            services.AddOptions();
            services.AddHttpClient<ResendClient>();
            services.Configure<ResendClientOptions>(options =>
            {
                options.ApiToken = smtpPassword;
            });
            services.AddTransient<IResend, ResendClient>();
            services.AddScoped<IEmailService, ResendEmailService>();
        }
        else
        {
            services.AddScoped<IEmailService, SmtpEmailService>();
        }
    }
}