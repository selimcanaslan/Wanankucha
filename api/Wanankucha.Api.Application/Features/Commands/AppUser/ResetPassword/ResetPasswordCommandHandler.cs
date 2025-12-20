using MediatR;
using Microsoft.Extensions.Logging;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Application.Wrappers;
using Wanankucha.Api.Domain.Repositories;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.ResetPassword;

public class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    ILogger<ResetPasswordCommandHandler> logger)
    : IRequestHandler<ResetPasswordCommandRequest, ServiceResponse<string>>
{
    public async Task<ServiceResponse<string>> Handle(ResetPasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToUpperInvariant();
        var user = await userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("Password reset attempted for non-existent email: {Email}", request.Email);
            return new ServiceResponse<string>("Invalid or expired reset token.")
            {
                Succeeded = false
            };
        }

        // Validate token
        if (user.PasswordResetToken != request.Token)
        {
            logger.LogWarning("Invalid password reset token for user {UserId}", user.Id);
            return new ServiceResponse<string>("Invalid or expired reset token.")
            {
                Succeeded = false
            };
        }

        // Check token expiry
        if (user.PasswordResetTokenExpiry == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
        {
            logger.LogWarning("Expired password reset token for user {UserId}", user.Id);
            return new ServiceResponse<string>("Invalid or expired reset token.")
            {
                Succeeded = false
            };
        }

        // Update password
        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;

        // Invalidate refresh tokens for security
        user.RefreshToken = null;
        user.RefreshTokenEndDate = null;

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Password successfully reset for user {UserId}", user.Id);

        return new ServiceResponse<string>("Password has been reset successfully. Please log in with your new password.")
        {
            Succeeded = true
        };
    }
}
