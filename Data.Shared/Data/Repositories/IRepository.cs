using System.Linq.Expressions;
using Models.Shared.Common;
using Models.Shared.Entities;
using NetBlocks.Models;

namespace Data.Shared.Data.Repositories;

public interface IRepository<TEntity> : ICrudRepository<TEntity>
where TEntity : class, IEntity
{
    
} 