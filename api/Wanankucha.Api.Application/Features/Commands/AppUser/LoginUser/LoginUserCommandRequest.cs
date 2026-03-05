using MediatR;
using Wanankucha.Api.Application.DTOs;
using Wanankucha.Api.Domain.Common;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.LoginUser;

public class LoginUserCommandRequest : IRequest<Result<Token>>
{
    public required string EmailOrUserName { get; set; }
    public required string Password { get; set; }
}
