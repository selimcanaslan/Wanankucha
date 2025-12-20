namespace Wanankucha.Api.Application.Features.Queries.AppUser.GetAllUsers;

public class GetAllUsersQueryResponse
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string NameSurname { get; set; }
    public required string UserName { get; set; }
}