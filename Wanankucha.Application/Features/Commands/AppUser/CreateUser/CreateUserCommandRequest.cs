using MediatR;
using Wanankucha.Application.Wrappers;

namespace Wanankucha.Application.Features.Commands.AppUser.CreateUser;

public class CreateUserCommandRequest : IRequest<ServiceResponse<Guid>>
{
    public required string NameSurname { get; set; }
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string PasswordConfirm { get; set; }
}