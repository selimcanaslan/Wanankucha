namespace Wanankucha.Api.Domain.Common;

public interface IEntity
{
    DateTime CreatedDate { get; set; }
    DateTime? UpdatedDate { get; set; }
    bool IsDeleted { get; set; }
}