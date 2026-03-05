using MediatR;
using Wanankucha.Api.Domain.Common;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.ResetPassword;

public record ResetPasswordCommandRequest(
    string Email,
    string Token,
    string NewPassword) : IRequest<Result<string>>;
