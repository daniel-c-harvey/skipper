using System.Linq.Expressions;
using Models.Shared.Common;
using Models.Shared.Entities;
using NetBlocks.Models;

namespace Data.Shared.Data.Repositories;

public interface ICompositeRepository<TComposite, TRoot, TDiscriminator, TInfo> : ICrudRepository<TComposite>
    where TComposite : class, ICompositeEntity<TRoot, TDiscriminator, TInfo>
    where TRoot : ICompositeEntityRoot<TDiscriminator>
    where TInfo : IEntity
{
    
}