using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Wanankucha.Api.Application.Features.Commands.AppUser.CreateUser;
using Wanankucha.Api.Application.Features.Queries.AppUser.GetAllUsers;

namespace Wanankucha.Api.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserCommandRequest request)
    {
        var response = await mediator.Send(request);

        if (response.Succeeded)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [Authorize]
    [HttpGet]
    [OutputCache(PolicyName = "Users")]
    public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersQueryRequest request)
    {
        var response = await mediator.Send(request);
        return Ok(response);
    }
}