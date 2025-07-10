using Models.Shared.Entities;

namespace Models.Shared.Models;

public abstract class BaseModel<TSelf, TEntity>
    where TSelf : class, IModel<TSelf, TEntity>
    where TEntity : class, IEntity<TEntity, TSelf>
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}