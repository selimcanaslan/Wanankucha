using MediatR;
using Wanankucha.Api.Application.RequestParameters;
using Wanankucha.Api.Domain.Common;

namespace Wanankucha.Api.Application.Features.Queries.AppUser.GetAllUsers;

public class GetAllUsersQueryRequest : IRequest<Result<List<GetAllUsersQueryResponse>>>
{
    public Pagination? Pagination { get; set; }
}
