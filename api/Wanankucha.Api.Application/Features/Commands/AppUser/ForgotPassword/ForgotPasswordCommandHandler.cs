using MediatR;
using Microsoft.Extensions.Logging;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Domain.Common;
using Wanankucha.Api.Domain.Repositories;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.ForgotPassword;

public class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    ILogger<ForgotPasswordCommandHandler> logger)
    : IRequestHandler<ForgotPasswordCommandRequest, Result<string>>
{
    public async Task<Result<string>> Handle(ForgotPasswordCommandRequest request, CancellationToken cancellationToken)
    {
        const string safeMessage = "If the email exists, a password reset link has been sent.";

        var normalizedEmail = request.Email.ToUpperInvariant();
        var user = await userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);

        if (user == null)
        {
            // Don't reveal that the user doesn't exist for security
            logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
            return Result<string>.Success(safeMessage);
        }

        // Domain method generates the token and sets expiry
        user.GeneratePasswordResetToken();

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await emailService.SendPasswordResetEmailAsync(user.Email, user.PasswordResetToken!, cancellationToken);
            logger.LogInformation("Password reset email sent to user {UserId}", user.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send password reset email to user {UserId}", user.Id);
            // Don't fail the request — token is saved, user can retry
        }

        return Result<string>.Success(safeMessage);
    }
}
