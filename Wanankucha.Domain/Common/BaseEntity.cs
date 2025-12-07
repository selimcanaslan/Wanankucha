namespace Wanankucha.Domain.Common;

public abstract class BaseEntity<TKey> : IEntity
{
    public TKey? Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
}