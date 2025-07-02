using SkipperModels.Entities;

namespace SkipperModels.Models;

public interface IModel<TSelf, TEntity>
    where TSelf : class, IModel<TSelf, TEntity>
    where TEntity : class, IEntity<TEntity, TSelf>
{
    long Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
    static abstract TEntity CreateEntity(TSelf dto);
}