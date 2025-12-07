using MediatR;
using Microsoft.AspNetCore.Identity;
using Wanankucha.Application.Wrappers;

namespace Wanankucha.Application.Features.Commands.AppUser.CreateUser;

public class CreateUserCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager)
    : IRequestHandler<CreateUserCommandRequest, ServiceResponse<Guid>>
{
    public async Task<ServiceResponse<Guid>> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
    {
        var user = new Domain.Entities.Identity.AppUser
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            Email = request.Email,
            NameSurname = request.NameSurname
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            return new ServiceResponse<Guid>(user.Id, "User created successfully.");
        }
        else
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Description;
            return new ServiceResponse<Guid>(errorMessage ?? "Something went wrong while creating the user.");
        }
    }
}