using MediatR;
using Wanankucha.Application.Abstractions;
using Wanankucha.Application.DTOs;
using Wanankucha.Application.Wrappers;

namespace Wanankucha.Application.Features.Commands.AppUser.RefreshToken;

public class RefreshTokenCommandHandler(
    IUserService userService,
    ITokenService tokenService)
    : IRequestHandler<RefreshTokenCommandRequest, ServiceResponse<Token>>
{
    public async Task<ServiceResponse<Token>> Handle(RefreshTokenCommandRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userService.FindByRefreshTokenAsync(request.RefreshToken);

        if (user == null || user.RefreshTokenEndDate < DateTime.UtcNow)
        {
            return new ServiceResponse<Token>("Refresh Token geçersiz veya süresi dolmuş.");
        }

        var token = tokenService.CreateAccessToken(user);

        await userService.UpdateRefreshTokenAsync(user.Id, token.RefreshToken, token.Expiration.AddDays(7).AddMinutes(-15));

        const string message = "Tokens are refreshed successfully!";
        return new ServiceResponse<Token>(token, message);
    }
}