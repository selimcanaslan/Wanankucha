namespace Wanankucha.Api.Domain.Common;

public interface IEntity
{
    DateTime CreatedDate { get; }
    DateTime? UpdatedDate { get; }
    bool IsDeleted { get; }
}