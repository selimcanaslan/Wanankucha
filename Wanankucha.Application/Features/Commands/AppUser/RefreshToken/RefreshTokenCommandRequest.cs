using MediatR;
using Wanankucha.Application.DTOs;
using Wanankucha.Application.Wrappers;

namespace Wanankucha.Application.Features.Commands.AppUser.RefreshToken;

public class RefreshTokenCommandRequest : IRequest<ServiceResponse<Token>>
{
    public string RefreshToken { get; set; }
}