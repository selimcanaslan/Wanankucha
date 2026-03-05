namespace Wanankucha.Api.Domain.Events;

using Wanankucha.Api.Domain.Common;

public record UserLockedOutDomainEvent(Guid UserId, string Email) : IDomainEvent;
