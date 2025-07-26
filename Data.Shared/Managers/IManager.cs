using Models.Shared.Entities;
using Models.Shared.Models;

namespace Data.Shared.Managers;

public interface IManager<TEntity, TModel> : IManagerBase<TEntity, TModel>
where TEntity : class, IEntity
where TModel : class, IModel
{
}