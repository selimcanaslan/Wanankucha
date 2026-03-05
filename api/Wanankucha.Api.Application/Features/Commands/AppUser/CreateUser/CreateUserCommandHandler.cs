using MediatR;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Domain.Common;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.CreateUser;

public class CreateUserCommandHandler(IUserService userService)
    : IRequestHandler<CreateUserCommandRequest, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
    {
        return await userService.CreateUserAsync(
            request.NameSurname,
            request.Email,
            request.UserName,
            request.Password,
            cancellationToken);
    }
}
