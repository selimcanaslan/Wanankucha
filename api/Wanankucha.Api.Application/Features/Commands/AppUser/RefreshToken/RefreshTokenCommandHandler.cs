using MediatR;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Application.DTOs;
using Wanankucha.Api.Application.Wrappers;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.RefreshToken;

public class RefreshTokenCommandHandler(
    IUserService userService,
    ITokenService tokenService)
    : IRequestHandler<RefreshTokenCommandRequest, ServiceResponse<Token>>
{
    public async Task<ServiceResponse<Token>> Handle(RefreshTokenCommandRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userService.FindByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (user == null || user.RefreshTokenEndDate < DateTime.UtcNow)
        {
            return new ServiceResponse<Token>("Refresh Token geçersiz veya süresi dolmuş.");
        }

        var token = tokenService.CreateAccessToken(user);

        await userService.UpdateRefreshTokenAsync(user.Id, token.RefreshToken, token.Expiration.AddDays(7).AddMinutes(-15), cancellationToken);

        const string message = "Tokens are refreshed successfully!";
        return new ServiceResponse<Token>(token, message);
    }
}