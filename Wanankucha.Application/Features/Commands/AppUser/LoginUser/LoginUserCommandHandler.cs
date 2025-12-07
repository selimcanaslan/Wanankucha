using MediatR;
using Wanankucha.Application.Abstractions;
using Wanankucha.Application.DTOs;
using Wanankucha.Application.Wrappers;

namespace Wanankucha.Application.Features.Commands.AppUser.LoginUser;

public class LoginUserCommandHandler(
    IUserService userService,
    ITokenService tokenService)
    : IRequestHandler<LoginUserCommandRequest, ServiceResponse<Token>>
{
    public async Task<ServiceResponse<Token>> Handle(LoginUserCommandRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userService.FindByEmailOrUsernameAsync(request.EmailOrUserName);

        if (user == null)
            return new ServiceResponse<Token>("Incorrect username or password");

        var checkPassword = await userService.CheckPasswordAsync(user.Id, request.Password);
        if (!checkPassword)
            return new ServiceResponse<Token>("Incorrect username or password");

        var token = tokenService.CreateAccessToken(user);
        
        await userService.UpdateRefreshTokenAsync(user.Id, token.RefreshToken, token.Expiration.AddDays(7));

        string message = "Authentication successful";
        return new ServiceResponse<Token>(token, message);
    }
}