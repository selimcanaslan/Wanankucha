using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Wanankucha.Api.Application.Features.Commands.AppUser.CreateUser;
using Wanankucha.Api.Application.Features.Commands.AppUser.ForgotPassword;
using Wanankucha.Api.Application.Features.Commands.AppUser.LoginUser;
using Wanankucha.Api.Application.Features.Commands.AppUser.RefreshToken;
using Wanankucha.Api.Application.Features.Commands.AppUser.ResetPassword;

namespace Wanankucha.Api.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("Login")]
    [EnableRateLimiting("auth")]
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

    [HttpPost("Register")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register(CreateUserCommandRequest request)
    {
        var response = await mediator.Send(request);

        if (response.Succeeded)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPost("ForgotPassword")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordCommandRequest request)
    {
        var response = await mediator.Send(request);
        return Ok(response); // Always return OK to prevent email enumeration
    }

    [HttpPost("ResetPassword")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommandRequest request)
    {
        var response = await mediator.Send(request);

        if (response.Succeeded)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}