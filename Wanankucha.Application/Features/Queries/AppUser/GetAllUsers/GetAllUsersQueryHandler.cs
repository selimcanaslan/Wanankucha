using MediatR;
using Wanankucha.Application.Abstractions;
using Wanankucha.Application.Wrappers;

namespace Wanankucha.Application.Features.Queries.AppUser.GetAllUsers;

public class GetAllUsersQueryHandler(IUserService userService)
    : IRequestHandler<GetAllUsersQueryRequest, ServiceResponse<List<GetAllUsersQueryResponse>>>
{
    public async Task<ServiceResponse<List<GetAllUsersQueryResponse>>> Handle(GetAllUsersQueryRequest request, CancellationToken cancellationToken)
    {
        var users = await userService.GetAllUsersAsync(request.Pagination.Page, request.Pagination.Size);

        var response = users.Select(user => new GetAllUsersQueryResponse
        {
            Id = user.Id.ToString(),
            Email = user.Email,
            NameSurname = user.NameSurname,
            UserName = user.UserName
        }).ToList();

        return new ServiceResponse<List<GetAllUsersQueryResponse>>(response);
    }
}