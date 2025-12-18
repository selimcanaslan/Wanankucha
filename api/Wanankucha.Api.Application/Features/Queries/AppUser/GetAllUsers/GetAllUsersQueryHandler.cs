using MediatR;
using Wanankucha.Api.Application.Abstractions;
using Wanankucha.Api.Application.Wrappers;

namespace Wanankucha.Api.Application.Features.Queries.AppUser.GetAllUsers;

public class GetAllUsersQueryHandler(IUserService userService)
    : IRequestHandler<GetAllUsersQueryRequest, ServiceResponse<List<GetAllUsersQueryResponse>>>
{
    public async Task<ServiceResponse<List<GetAllUsersQueryResponse>>> Handle(GetAllUsersQueryRequest request, CancellationToken cancellationToken)
    {
        var pagination = request.Pagination ?? new RequestParameters.Pagination();
        var users = await userService.GetAllUsersAsync(pagination.Page, pagination.Size, cancellationToken);

        var response = users.Select(user => new GetAllUsersQueryResponse
        {
            Id = user.Id.ToString(),
            Email = user.Email ?? string.Empty,
            NameSurname = user.NameSurname,
            UserName = user.UserName ?? string.Empty
        }).ToList();

        return new ServiceResponse<List<GetAllUsersQueryResponse>>(response);
    }
}