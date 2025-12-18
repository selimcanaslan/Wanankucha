using MediatR;
using Wanankucha.Api.Application.RequestParameters;
using Wanankucha.Api.Application.Wrappers;

namespace Wanankucha.Api.Application.Features.Queries.AppUser.GetAllUsers;

public class GetAllUsersQueryRequest : IRequest<ServiceResponse<List<GetAllUsersQueryResponse>>>
{
    public Pagination? Pagination { get; set; }
}