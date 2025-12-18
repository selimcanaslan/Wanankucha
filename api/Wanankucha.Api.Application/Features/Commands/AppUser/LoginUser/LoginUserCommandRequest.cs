using MediatR;
using Wanankucha.Api.Application.DTOs;
using Wanankucha.Api.Application.Wrappers;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.LoginUser;

public class LoginUserCommandRequest : IRequest<ServiceResponse<Token>>
{
    public required string EmailOrUserName { get; set; }
    public required string Password { get; set; }
}