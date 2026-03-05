namespace Wanankucha.Api.Application.Abstractions;

/// <summary>
/// Data required by the token service to generate a JWT.
/// Keeps the Infrastructure layer decoupled from the Domain entity.
/// </summary>
public record UserTokenData(Guid Id, string UserName, string Email);
