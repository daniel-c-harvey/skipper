using SkipperModels.Entities;
using SkipperModels.Models;

namespace SkipperModels.InputModels;

public interface IInputModel<TInput, TModel, TEntity>
where TInput : class, IInputModel<TInput, TModel, TEntity>
where TModel : class, IModel<TModel, TEntity>
where TEntity : class, IEntity<TEntity, TModel>

{
    static abstract TModel MakeModel(TInput input);
}