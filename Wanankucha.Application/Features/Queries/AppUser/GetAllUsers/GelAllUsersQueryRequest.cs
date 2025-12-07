using MediatR;
using Wanankucha.Application.RequestParameters;
using Wanankucha.Application.Wrappers;

namespace Wanankucha.Application.Features.Queries.AppUser.GetAllUsers;

public class GetAllUsersQueryRequest : IRequest<ServiceResponse<List<GetAllUsersQueryResponse>>>
{
    public Pagination? Pagination { get; set; }
}