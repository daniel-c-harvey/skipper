
namespace Models.Shared.Models;

public interface IModel
{
    long Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}