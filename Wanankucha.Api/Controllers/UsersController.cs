using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wanankucha.Application.Features.Commands.AppUser.CreateUser;
using Wanankucha.Application.Features.Queries.AppUser.GetAllUsers;

namespace Wanankucha.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
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
    public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersQueryRequest request)
    {
        var response = await mediator.Send(request);
        return Ok(response);
        
    }
}