
namespace Models.Shared.Entities;

public interface IEntity
{
    long Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
    bool IsDeleted { get; set; }
}