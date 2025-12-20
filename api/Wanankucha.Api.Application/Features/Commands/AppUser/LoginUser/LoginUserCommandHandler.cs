using MediatR;
using Microsoft.Extensions.Logging;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Application.DTOs;
using Wanankucha.Api.Application.Wrappers;
using Wanankucha.Api.Domain.Repositories;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.LoginUser;

public class LoginUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    ILogger<LoginUserCommandHandler> logger)
    : IRequestHandler<LoginUserCommandRequest, ServiceResponse<Token>>
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    public async Task<ServiceResponse<Token>> Handle(LoginUserCommandRequest request,
        CancellationToken cancellationToken)
    {
        var normalized = request.EmailOrUserName.ToUpperInvariant();
        var user = await userRepository.FindByEmailOrUsernameAsync(normalized, cancellationToken);

        if (user == null)
            return new ServiceResponse<Token>("Incorrect username or password");

        // Check if account is locked
        if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            var remainingTime = user.LockoutEnd.Value - DateTime.UtcNow;
            logger.LogWarning("Login attempt for locked account {UserId}. Lockout ends in {Minutes} minutes", 
                user.Id, remainingTime.TotalMinutes);
            return new ServiceResponse<Token>(
                $"Account is locked due to too many failed attempts. Please try again in {Math.Ceiling(remainingTime.TotalMinutes)} minutes, or use 'Forgot Password' to reset your password.");
        }

        // Verify password
        var isPasswordValid = passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        
        if (!isPasswordValid)
        {
            // Increment failed login attempts
            user.FailedLoginAttempts++;
            
            if (user.LockoutEnabled && user.FailedLoginAttempts >= MaxFailedAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.Add(LockoutDuration);
                logger.LogWarning("Account {UserId} locked after {Attempts} failed attempts", 
                    user.Id, user.FailedLoginAttempts);
            }
            
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            return new ServiceResponse<Token>("Incorrect username or password");
        }

        // Reset failed attempts on successful login
        if (user.FailedLoginAttempts > 0 || user.LockoutEnd.HasValue)
        {
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
        }

        // Generate tokens
        var token = tokenService.CreateAccessToken(user);
        
        // Update refresh token
        user.RefreshToken = token.RefreshToken;
        user.RefreshTokenEndDate = token.Expiration.AddDays(7);
        
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} logged in successfully", user.Id);
        return new ServiceResponse<Token>(token, "Authentication successful");
    }
}