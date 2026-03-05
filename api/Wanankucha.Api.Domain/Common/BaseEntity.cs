namespace Wanankucha.Api.Domain.Common;

public abstract class BaseEntity<TKey> : IEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public TKey? Id { get; protected set; }
    public DateTime CreatedDate { get; protected set; }
    public DateTime? UpdatedDate { get; protected set; }
    public bool IsDeleted { get; protected set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}