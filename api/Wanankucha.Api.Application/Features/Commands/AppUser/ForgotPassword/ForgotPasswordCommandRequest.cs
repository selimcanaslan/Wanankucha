using MediatR;
using Wanankucha.Api.Domain.Common;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.ForgotPassword;

public record ForgotPasswordCommandRequest(string Email) : IRequest<Result<string>>;
