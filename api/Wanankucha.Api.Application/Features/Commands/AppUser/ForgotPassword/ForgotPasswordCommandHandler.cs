using MediatR;
using Microsoft.Extensions.Logging;
using Wanankucha.Api.Application.Wrappers;
using Wanankucha.Api.Domain.Repositories;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.ForgotPassword;

public class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
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

        // TODO: Send email with reset link containing the token
        // For now, log the token (in production, this should be sent via email)
        logger.LogInformation(
            "Password reset token generated for user {UserId}: {Token} (expires: {Expiry})",
            user.Id, resetToken, user.PasswordResetTokenExpiry);

        // In a real app, you would send an email here with a link like:
        // https://yourapp.com/reset-password?email={email}&token={resetToken}
        
        return new ServiceResponse<string>("If the email exists, a password reset link has been sent.")
        {
            Succeeded = true,
            Data = resetToken // Only for development - remove in production!
        };
    }
}
