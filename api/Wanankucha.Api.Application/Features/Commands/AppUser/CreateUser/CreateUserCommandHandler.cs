using MediatR;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Application.Wrappers;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.CreateUser;

public class CreateUserCommandHandler(IUserService userService)
    : IRequestHandler<CreateUserCommandRequest, ServiceResponse<Guid>>
{
    public async Task<ServiceResponse<Guid>> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
    {
        return await userService.CreateUserAsync(
            request.NameSurname,
            request.Email,
            request.UserName,
            request.Password,
            cancellationToken);
    }
}