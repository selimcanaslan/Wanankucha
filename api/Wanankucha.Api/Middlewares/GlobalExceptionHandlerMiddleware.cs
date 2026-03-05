using System.Net;
using System.Text.Json;
using FluentValidation;
using Wanankucha.Api.Domain.Common;

namespace Wanankucha.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception error)
        {
            logger.LogError(error, error.Message);
            await HandleExceptionAsync(context, error);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception error)
    {
        context.Response.ContentType = "application/json";

        var statusCode = (int)HttpStatusCode.InternalServerError;
        var errorList = new List<Error>();

        switch (error)
        {
            case Application.Exceptions.NotFoundException notFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                errorList.Add(Error.NotFound("Record.NotFound", notFoundException.Message));
                break;
            case Domain.Exceptions.DomainException domainException:
                statusCode = (int)HttpStatusCode.Conflict;
                errorList.Add(Error.Conflict("Domain.RuleViolation", domainException.Message));
                break;
            case ValidationException validationException:
                statusCode = (int)HttpStatusCode.BadRequest;
                errorList.AddRange(validationException.Errors.Select(x => Error.Validation("Validation.Error", x.ErrorMessage)));
                break;
            default:
                errorList.Add(Error.Failure("Internal.Error", "Internal Server Error"));
                break;
        }

        var response = Result<string>.Failure(errorList);

        context.Response.StatusCode = statusCode;

        var jsonResult = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResult);
    }
}
