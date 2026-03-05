using MediatR;
using Wanankucha.Api.Application.DTOs;
using Wanankucha.Api.Domain.Common;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.RefreshToken;

public class RefreshTokenCommandRequest : IRequest<Result<Token>>
{
    public required string RefreshToken { get; set; }
}
