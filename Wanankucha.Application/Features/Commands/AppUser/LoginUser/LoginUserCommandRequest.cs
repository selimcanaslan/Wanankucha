using MediatR;
using Wanankucha.Application.DTOs;
using Wanankucha.Application.Wrappers;

namespace Wanankucha.Application.Features.Commands.AppUser.LoginUser;

public class LoginUserCommandRequest : IRequest<ServiceResponse<Token>>
{
    public required string EmailOrUserName { get; set; }
    public required string Password { get; set; }
}