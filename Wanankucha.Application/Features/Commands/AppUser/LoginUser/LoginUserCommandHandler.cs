using MediatR;
using Microsoft.AspNetCore.Identity;
using Wanankucha.Application.Abstractions;
using Wanankucha.Application.DTOs;
using Wanankucha.Application.Wrappers;

namespace Wanankucha.Application.Features.Commands.AppUser.LoginUser;

public class LoginUserCommandHandler(
    UserManager<Domain.Entities.Identity.AppUser> userManager,
    ITokenService tokenService)
    : IRequestHandler<LoginUserCommandRequest, ServiceResponse<Token>>
{
    public async Task<ServiceResponse<Token>> Handle(LoginUserCommandRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.EmailOrUserName);
        if (user == null)
            user = await userManager.FindByNameAsync(request.EmailOrUserName);

        if (user == null)
            return new ServiceResponse<Token>("Incorrect username or password");

        var checkPassword = await userManager.CheckPasswordAsync(user, request.Password);
        if (!checkPassword)
            return new ServiceResponse<Token>("Incorrect username or password");

        var token = tokenService.CreateAccessToken(user);
        
        user.RefreshToken = token.RefreshToken;
        user.RefreshTokenEndDate = token.Expiration.AddDays(7);

        await userManager.UpdateAsync(user);

        string message = "Authentication successful";
        return new ServiceResponse<Token>(token, message);
    }
}