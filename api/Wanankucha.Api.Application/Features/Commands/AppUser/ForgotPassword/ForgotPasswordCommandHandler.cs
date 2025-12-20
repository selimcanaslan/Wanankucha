using MediatR;
using Microsoft.Extensions.Logging;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Application.Wrappers;
using Wanankucha.Api.Domain.Repositories;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.ForgotPassword;

public class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    ILogger<ForgotPasswordCommandHandler> logger)
    : IRequestHandler<ForgotPasswordCommandRequest, ServiceResponse<string>>
{
    public async Task<ServiceResponse<string>> Handle(ForgotPasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToUpperInvariant();
        var user = await userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);

        if (user == null)
        {
            // Don't reveal that the user doesn't exist for security
            logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
            return new ServiceResponse<string>("If the email exists, a password reset link has been sent.")
            {
                Succeeded = true
            };
        }

        // Generate reset token (secure random string)
        var resetToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Send password reset email
        try
        {
            await emailService.SendPasswordResetEmailAsync(user.Email, resetToken, cancellationToken);
            logger.LogInformation("Password reset email sent to user {UserId}", user.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send password reset email to user {UserId}", user.Id);
            // Don't fail the request - token is saved, user can retry
        }

        return new ServiceResponse<string>("If the email exists, a password reset link has been sent.")
        {
            Succeeded = true
        };
    }
}
