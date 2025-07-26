using Data.Shared.Data.Repositories;
using Models.Shared.Converters;
using Models.Shared.Entities;
using Models.Shared.Models;

namespace Data.Shared.Managers;

public abstract class Manager<TEntity, TModel, TRepository, TConverter> 
    : ManagerBase<TEntity, TModel, TRepository, TConverter>, 
    IManager<TEntity, TModel>
    where TEntity : class, IEntity
    where TModel : class, IModel, new()
    where TRepository : IRepository<TEntity>
    where TConverter : IEntityToModelConverter<TEntity, TModel>
{
    protected Manager(TRepository repository) : base(repository)
    {
    }
}