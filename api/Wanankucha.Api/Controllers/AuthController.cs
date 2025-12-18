using MediatR;
using Microsoft.AspNetCore.Mvc;
using Wanankucha.Api.Application.Features.Commands.AppUser.LoginUser;
using Wanankucha.Api.Application.Features.Commands.AppUser.RefreshToken;

namespace Wanankucha.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginUserCommandRequest request)
    {
        var response = await mediator.Send(request);

        if (response.Succeeded)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
    
    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken(RefreshTokenCommandRequest request)
    {
        var response = await mediator.Send(request);
    
        if (response.Succeeded)
        {
            return Ok(response);
        }
    
        return BadRequest(response);
    }
}