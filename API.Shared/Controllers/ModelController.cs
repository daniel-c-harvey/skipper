using Data.Shared.Managers;
using Microsoft.AspNetCore.Mvc;
using Models.Shared.Entities;
using Models.Shared.Models;

namespace API.Shared.Controllers
{
    public abstract class ModelController<TEntity, TModel, TManager> 
    : ModelControllerBase<TEntity, TModel, TManager>, IModelController<TModel> 
    where TEntity : class, IEntity, new()
    where TModel : class, IModel, new()
    where TManager : IManager<TEntity, TModel>
    {
        protected ModelController(TManager manager) : base(manager)
        {
            SortExpressions[nameof(IEntity.Id)] = e => e.Id;
            SortExpressions[nameof(IEntity.CreatedAt)] = e => e.CreatedAt;
            SortExpressions[nameof(IEntity.UpdatedAt)] = e => e.UpdatedAt;
        }
    }
} 