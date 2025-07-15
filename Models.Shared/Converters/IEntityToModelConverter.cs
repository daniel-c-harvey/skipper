using Models.Shared.Entities;
using Models.Shared.Models;

namespace Models.Shared.Converters;

public interface IEntityToModelConverter<TEntity, TModel>
    where TEntity : class, IEntity
    where TModel : class, IModel, new()
{
    static abstract TModel Convert(TEntity entity);
    static abstract TEntity Convert(TModel model);
}
