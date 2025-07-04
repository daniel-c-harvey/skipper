using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperModels.InputModels;

public interface IInputModel<TInput, TModel, TEntity>
where TInput : class, IInputModel<TInput, TModel, TEntity>
where TModel : class, IModel<TModel, TEntity>
where TEntity : class, IEntity<TEntity, TModel>

{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    static abstract TModel MakeModel(TInput input);
    static abstract TInput From(TModel model);
}