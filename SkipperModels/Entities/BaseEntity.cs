
using SkipperModels.Models;

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