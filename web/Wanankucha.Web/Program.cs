using Microsoft.AspNetCore.Components.Authorization;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Wanankucha.Web.Auth;
using Wanankucha.Web.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting Wanankucha Web...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, configuration) => 
        configuration.ReadFrom.Configuration(context.Configuration));

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    // Configure HttpClient with resilience policies
    builder.Services.AddHttpClient("WanankuchaApi", client =>
    {
        var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] 
            ?? throw new InvalidOperationException("ApiSettings:BaseUrl not configured");
        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.Timeout = TimeSpan.FromSeconds(30); // Default timeout
    })
    .AddPolicyHandler((serviceProvider, _) =>
    {
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("HttpClient.Resilience");
        
        return HttpPolicyExtensions
            .HandleTransientHttpError() // Handles 5xx, 408, and network failures
            .WaitAndRetryAsync(3, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff: 2s, 4s, 8s
                onRetry: (outcome, timespan, retryAttempt, _) =>
                {
                    logger.LogWarning(
                        "Retry {RetryAttempt} after {Delay}s due to {Reason}",
                        retryAttempt,
                        timespan.TotalSeconds,
                        outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                });
    })
    .AddPolicyHandler((serviceProvider, _) =>
    {
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("HttpClient.Resilience");
        
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5, // Open circuit after 5 consecutive failures
                durationOfBreak: TimeSpan.FromSeconds(30), // Keep circuit open for 30s
                onBreak: (outcome, timespan) =>
                {
                    logger.LogError(
                        "Circuit breaker opened for {Duration}s due to {Reason}",
                        timespan.TotalSeconds,
                        outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                },
                onReset: () => logger.LogInformation("Circuit breaker reset"));
    });

    // Authentication services
    builder.Services.AddScoped<ITokenStorageService, CookieTokenStorageService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "BlazorServer";
        options.DefaultChallengeScheme = "BlazorServer";
    })
    .AddCookie("BlazorServer", options =>
    {
        options.LoginPath = "/login";
    });

    builder.Services.AddAuthorizationCore();
    builder.Services.AddCascadingAuthenticationState();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseAntiforgery();

    app.MapRazorComponents<Wanankucha.Web.Components.App>()
        .AddInteractiveServerRenderMode();

    Log.Information("Wanankucha Web started successfully");
    Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);

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

