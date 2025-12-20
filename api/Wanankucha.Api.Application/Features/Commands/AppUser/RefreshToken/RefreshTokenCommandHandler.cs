using MediatR;
using Microsoft.Extensions.Logging;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Application.DTOs;
using Wanankucha.Api.Application.Wrappers;
using Wanankucha.Api.Domain.Repositories;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.RefreshToken;

public class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    ILogger<RefreshTokenCommandHandler> logger)
    : IRequestHandler<RefreshTokenCommandRequest, ServiceResponse<Token>>
{
    public async Task<ServiceResponse<Token>> Handle(RefreshTokenCommandRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (user == null || user.RefreshTokenEndDate < DateTime.UtcNow)
        {
            logger.LogWarning("Invalid or expired refresh token attempted");
            return new ServiceResponse<Token>("Invalid or expired refresh token.");
        }

        // Token rotation: generate new access and refresh tokens
        var token = tokenService.CreateAccessToken(user);

        // Update refresh token (rotation)
        user.RefreshToken = token.RefreshToken;
        user.RefreshTokenEndDate = token.Expiration.AddDays(7);

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Token refreshed for user {UserId}", user.Id);
        return new ServiceResponse<Token>(token, "Tokens refreshed successfully!");
    }
}