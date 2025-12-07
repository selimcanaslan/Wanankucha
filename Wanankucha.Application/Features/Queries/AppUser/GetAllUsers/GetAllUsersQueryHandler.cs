using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wanankucha.Application.Wrappers;

namespace Wanankucha.Application.Features.Queries.AppUser.GetAllUsers;

public class GetAllUsersQueryHandler(UserManager<Domain.Entities.Identity.AppUser> userManager)
    : IRequestHandler<GetAllUsersQueryRequest, ServiceResponse<List<GetAllUsersQueryResponse>>>
{
    public async Task<ServiceResponse<List<GetAllUsersQueryResponse>>> Handle(GetAllUsersQueryRequest request, CancellationToken cancellationToken)
    {
        var query = userManager.Users.AsQueryable();

        var users = await query
            .Skip(request.Pagination.Page * request.Pagination.Size)
            .Take(request.Pagination.Size)
            .Select(user => new GetAllUsersQueryResponse
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                NameSurname = user.NameSurname,
                UserName = user.UserName
            })
            .ToListAsync(cancellationToken);

        return new ServiceResponse<List<GetAllUsersQueryResponse>>(users);
    }
}