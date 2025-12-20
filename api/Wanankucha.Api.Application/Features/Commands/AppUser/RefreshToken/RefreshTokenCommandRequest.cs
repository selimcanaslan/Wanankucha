using MediatR;
using Wanankucha.Api.Application.DTOs;
using Wanankucha.Api.Application.Wrappers;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.RefreshToken;

public class RefreshTokenCommandRequest : IRequest<ServiceResponse<Token>>
{
    public required string RefreshToken { get; set; }
}