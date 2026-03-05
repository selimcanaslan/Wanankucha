using MediatR;
using Microsoft.Extensions.Logging;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Domain.Common;
using Wanankucha.Api.Domain.Repositories;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.ResetPassword;

public class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    ILogger<ResetPasswordCommandHandler> logger)
    : IRequestHandler<ResetPasswordCommandRequest, Result<string>>
{
    public async Task<Result<string>> Handle(ResetPasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToUpperInvariant();
        var user = await userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("Password reset attempted for non-existent email: {Email}", request.Email);
            return Result<string>.Failure(Error.NotFound("User.NotFound", "User not found"));
        }

        // Domain method validates the token and its expiry
        if (!user.IsPasswordResetTokenValid(request.Token))
        {
            logger.LogWarning("Invalid or expired password reset token for user {UserId}", user.Id);
            return Result<string>.Failure(Error.Conflict("Token.Invalid", "Invalid or expired password reset token."));
        }

        // Update password via domain method
        user.SetPassword(passwordHasher.HashPassword(request.NewPassword));
        user.ClearPasswordResetToken();
        user.RevokeRefreshToken(); // Invalidate sessions for security

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Password successfully reset for user {UserId}", user.Id);
        return Result<string>.Success("Password has been reset successfully. Please log in with your new password.");
    }
}
