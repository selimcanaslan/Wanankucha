using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Infrastructure.Options;

namespace Wanankucha.Api.Infrastructure.Services.Email;

/// <summary>
/// Resend-based email service implementation (HTTP API - works on Railway)
/// </summary>
public class ResendEmailService : IEmailService
{
    private readonly SmtpOptions _options;
    private readonly ILogger<ResendEmailService> _logger;
    private readonly IResend _resend;
    private readonly bool _isConfigured;

    public ResendEmailService(
        IOptions<SmtpOptions> options, 
        ILogger<ResendEmailService> logger,
        IResend resend)
    {
        _options = options.Value;
        _logger = logger;
        _resend = resend;
        
        // Check if Resend API key is configured (we reuse Smtp__Password for the API key)
        _isConfigured = !string.IsNullOrEmpty(_options.Password) && _options.Password.StartsWith("re_");

        if (!_isConfigured)
        {
            _logger.LogWarning("Resend is not configured. Emails will be logged but not sent. " +
                "Set Smtp__Password to your Resend API key (starts with 're_').");
        }
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        if (!_isConfigured)
        {
            _logger.LogInformation(
                "📧 EMAIL (not sent - Resend not configured)\n" +
                "   To: {To}\n" +
                "   Subject: {Subject}\n" +
                "   Body: {Body}",
                to, subject, htmlBody);
            return;
        }

        try
        {
            var message = new EmailMessage
            {
                From = $"{_options.FromName} <{_options.FromEmail}>",
                To = to,
                Subject = subject,
                HtmlBody = htmlBody
            };

            await _resend.EmailSendAsync(message, cancellationToken);

            _logger.LogInformation("Email sent successfully to {To} with subject '{Subject}'", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetToken, CancellationToken cancellationToken = default)
    {
        var resetUrl = $"{_options.WebAppBaseUrl}/reset-password?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(to)}";

        var subject = "Reset Your Password - Wanankucha";
        var htmlBody = $$"""
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                    .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                    .button { 
                        display: inline-block; 
                        padding: 12px 24px; 
                        background-color: #4F46E5; 
                        color: white !important; 
                        text-decoration: none; 
                        border-radius: 6px; 
                        margin: 20px 0;
                    }
                    .footer { margin-top: 30px; font-size: 12px; color: #666; }
                </style>
            </head>
            <body>
                <div class="container">
                    <h2>Password Reset Request</h2>
                    <p>We received a request to reset your password for your Wanankucha account.</p>
                    <p>Click the button below to reset your password:</p>
                    <a href="{{resetUrl}}" class="button">Reset Password</a>
                    <p>Or copy and paste this link into your browser:</p>
                    <p style="word-break: break-all; color: #4F46E5;">{{resetUrl}}</p>
                    <p><strong>This link will expire in 1 hour.</strong></p>
                    <p>If you didn't request a password reset, you can safely ignore this email.</p>
                    <div class="footer">
                        <p>© Wanankucha - This is an automated message, please do not reply.</p>
                    </div>
                </div>
            </body>
            </html>
            """;

        await SendEmailAsync(to, subject, htmlBody, cancellationToken);
    }
}
