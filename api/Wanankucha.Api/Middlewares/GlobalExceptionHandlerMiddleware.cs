using System.Net;
using System.Text.Json;
using FluentValidation;
using Wanankucha.Api.Application.Wrappers;

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
            var response = new ServiceResponse<string>(error.Message)
            {
                Succeeded = false
            };

            switch (error)
            {
                case ValidationException validationException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response.Errors = validationException.Errors.Select(x => x.ErrorMessage).ToList();
                    response.Message = "Validation Error";
                    break;
                default:
                    response.Message = "Internal Server Error";
                    break;
            }

            context.Response.StatusCode = statusCode;

            var jsonResult = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResult);
        }
    }