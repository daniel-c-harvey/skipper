
namespace SkipperModels.Entities;

public abstract class BaseEntity<TSelf, TModel>
where TSelf : class, IEntity<TSelf, TModel>
where TModel : class, IModel<TModel, TSelf>, new()
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}

public abstract class BaseModel<TSelf, TEntity>
where TSelf : class, IModel<TSelf, TEntity>
where TEntity : class, IEntity<TEntity, TSelf>
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}