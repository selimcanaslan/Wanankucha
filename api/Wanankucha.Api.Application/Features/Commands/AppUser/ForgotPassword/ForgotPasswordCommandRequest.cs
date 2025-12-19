using MediatR;
using Wanankucha.Api.Application.Wrappers;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.ForgotPassword;

public record ForgotPasswordCommandRequest(string Email) : IRequest<ServiceResponse<string>>;
