namespace Wanankucha.Api.Application.Abstractions;

/// <summary>
/// Email service abstraction for sending emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email asynchronously
    /// </summary>
    Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sends a password reset email with the reset link
    /// </summary>
    Task SendPasswordResetEmailAsync(string to, string resetToken, CancellationToken cancellationToken = default);
}
