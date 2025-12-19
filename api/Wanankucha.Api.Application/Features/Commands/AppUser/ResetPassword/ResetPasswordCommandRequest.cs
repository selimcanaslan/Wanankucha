using MediatR;
using Wanankucha.Api.Application.Wrappers;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.ResetPassword;

public record ResetPasswordCommandRequest(
    string Email,
    string Token,
    string NewPassword) : IRequest<ServiceResponse<string>>;
