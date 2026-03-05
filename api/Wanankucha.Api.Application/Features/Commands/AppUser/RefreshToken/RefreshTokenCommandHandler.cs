using MediatR;
using Microsoft.Extensions.Logging;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Application.DTOs;
using Wanankucha.Api.Domain.Common;
using Wanankucha.Api.Domain.Repositories;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.RefreshToken;

public class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    ILogger<RefreshTokenCommandHandler> logger)
    : IRequestHandler<RefreshTokenCommandRequest, Result<Token>>
{
    public async Task<Result<Token>> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (user == null || user.RefreshTokenEndDate < DateTime.UtcNow)
        {
            logger.LogWarning("Invalid or expired refresh token attempted");
            return Result<Token>.Failure(Error.NotFound("Token.Invalid", "Invalid or expired refresh token."));
        }

        // Token rotation: generate new access and refresh tokens
        var tokenData = new UserTokenData(user.Id, user.UserName, user.Email);
        var token = tokenService.CreateAccessToken(tokenData);

        // Update via domain method
        user.SetRefreshToken(token.RefreshToken, token.Expiration.AddDays(7));

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Token refreshed for user {UserId}", user.Id);
        return Result<Token>.Success(token);
    }
}
