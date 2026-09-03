using MediatR;
using Microsoft.Extensions.Logging;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Domain.Events;

namespace Wanankucha.Api.Application.Features.EventHandlers;

/// <summary>
/// Handler for when a user account is locked out.
/// Responds to the domain event by logging and sending an email notification.
/// </summary>
public class UserLockedOutEventHandler(ILogger<UserLockedOutEventHandler> logger, IEmailService emailService) : INotificationHandler<UserLockedOutDomainEvent>
{
    private readonly ILogger<UserLockedOutEventHandler> _logger = logger;
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(UserLockedOutDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Account locked out for User: {UserId}, Email: {Email}", 
            notification.UserId, notification.Email);

        try
        {
            await _emailService.SendEmailAsync(
                notification.Email,
                "Security Alert: Your account has been locked",
                "Your account has been locked due to multiple failed login attempts. Please try again in 15 minutes or contact support.",
                cancellationToken);
            
            _logger.LogInformation("Security notification email sent to {Email}", notification.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send security notification email to {Email}", notification.Email);
        }
    }
}
