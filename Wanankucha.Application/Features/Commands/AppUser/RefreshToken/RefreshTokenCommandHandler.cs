using MediatR;
using Microsoft.AspNetCore.Identity;
using Wanankucha.Application.Abstractions;
using Wanankucha.Application.DTOs;
using Wanankucha.Application.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Wanankucha.Application.Features.Commands.AppUser.RefreshToken;

public class RefreshTokenCommandHandler(
    UserManager<Domain.Entities.Identity.AppUser> userManager,
    ITokenService tokenService)
    : IRequestHandler<RefreshTokenCommandRequest, ServiceResponse<Token>>
{
    public async Task<ServiceResponse<Token>> Handle(RefreshTokenCommandRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken: cancellationToken);

        if (user == null || user.RefreshTokenEndDate < DateTime.UtcNow)
        {
            return new ServiceResponse<Token>("Refresh Token geçersiz veya süresi dolmuş.");
        }

        var token = tokenService.CreateAccessToken(user);

        user.RefreshToken = token.RefreshToken;
        user.RefreshTokenEndDate = token.Expiration.AddDays(7);

        await userManager.UpdateAsync(user);

        const string message = "Tokens are refreshed successfully!";
        return new ServiceResponse<Token>(token, message);
    }
}