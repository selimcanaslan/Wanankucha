using Microsoft.AspNetCore.Mvc;
using Wanankucha.Api.Domain.Common;

namespace Wanankucha.Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result);
        }

        if (result.Error == null)
        {
            return BadRequest(result);
        }

        return result.Error.Code switch
        {
            var code when code.StartsWith("Record.NotFound") || code.StartsWith("User.NotFound") || code.StartsWith("Token.Invalid") => NotFound(result),
            var code when code.StartsWith("User.LockedOut") || code.StartsWith("User.Duplicate") => Conflict(result),
            var code when code.StartsWith("Validation") => BadRequest(result),
            _ => BadRequest(result)
        };
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok();
        }

        if (result.Error == null)
        {
            return BadRequest(result);
        }

        return result.Error.Code switch
        {
            var code when code.StartsWith("Record.NotFound") || code.StartsWith("User.NotFound") || code.StartsWith("Token.Invalid") => NotFound(result),
            var code when code.StartsWith("User.LockedOut") || code.StartsWith("User.Duplicate") => Conflict(result),
            var code when code.StartsWith("Validation") => BadRequest(result),
            _ => BadRequest(result)
        };
    }
}
