namespace Wanankucha.Api.Infrastructure.Options;

/// <summary>
/// SMTP configuration options
/// Configure these in appsettings.json under "Smtp" section:
/// {
///   "Smtp": {
///     "Host": "smtp.gmail.com",
///     "Port": 587,
///     "Username": "your-email@gmail.com",
///     "Password": "your-app-password",
///     "FromEmail": "noreply@wanankucha.com",
///     "FromName": "Wanankucha",
///     "EnableSsl": true
///   }
/// }
/// </summary>
public class SmtpOptions
{
    public const string SectionName = "Smtp";
    
    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = "noreply@wanankucha.com";
    public string FromName { get; set; } = "Wanankucha";
    public bool EnableSsl { get; set; } = true;
    
    /// <summary>
    /// Base URL for password reset links (e.g., https://localhost:5001)
    /// </summary>
    public string WebAppBaseUrl { get; set; } = "https://localhost:5001";
}
