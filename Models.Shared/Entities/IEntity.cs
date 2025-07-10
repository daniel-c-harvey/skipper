using Models.Shared.Models;

namespace Models.Shared.Entities;

public interface IEntity<TSelf, TModel>
where TSelf : class, IEntity<TSelf, TModel>
where TModel : class, IModel<TModel, TSelf>
{
    long Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
    bool IsDeleted { get; set; }
    static abstract TModel CreateModel(TSelf entity);
}