using Microsoft.AspNetCore.Components.Authorization;
using Wanankucha.Web.Auth;
using Wanankucha.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure HttpClient for API calls
builder.Services.AddHttpClient("WanankuchaApi", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] 
        ?? throw new InvalidOperationException("ApiSettings:BaseUrl not configured");
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
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

app.Run();
