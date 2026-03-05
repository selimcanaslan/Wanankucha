using MediatR;
using Microsoft.Extensions.Logging;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Application.DTOs;
using Wanankucha.Api.Domain.Common;
using Wanankucha.Api.Domain.Repositories;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.LoginUser;

public class LoginUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    ILogger<LoginUserCommandHandler> logger)
    : IRequestHandler<LoginUserCommandRequest, Result<Token>>
{
    public async Task<Result<Token>> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
    {
        var normalized = request.EmailOrUserName.ToUpperInvariant();
        var user = await userRepository.FindByEmailOrUsernameAsync(normalized, cancellationToken);

        if (user == null)
            return Result<Token>.Failure(Error.NotFound("User.NotFound", "Incorrect username or password"));

        // Check if account is locked — business rule lives on the entity
        if (user.IsLockedOut())
        {
            var remaining = user.LockoutEnd!.Value - DateTime.UtcNow;
            logger.LogWarning("Login attempt for locked account {UserId}. Lockout ends in {Minutes} minutes",
                user.Id, remaining.TotalMinutes);
            return Result<Token>.Failure(Error.Conflict("User.LockedOut",
                $"Account is locked. Please try again in {Math.Ceiling(remaining.TotalMinutes)} minutes, or use 'Forgot Password'."));
        }

        // Verify password
        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin(); // domain method
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Token>.Failure(Error.NotFound("User.NotFound", "Incorrect username or password"));
        }

        // Reset failed attempts on successful login
        if (user.FailedLoginAttempts > 0 || user.LockoutEnd.HasValue)
            user.ResetLoginAttempts(); // domain method

        // Generate tokens — pass DTO, not entity
        var tokenData = new UserTokenData(user.Id, user.UserName, user.Email);
        var token = tokenService.CreateAccessToken(tokenData);

        // Update refresh token via domain method
        user.SetRefreshToken(token.RefreshToken, token.Expiration.AddDays(7));

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} logged in successfully", user.Id);
        return Result<Token>.Success(token);
    }
}
