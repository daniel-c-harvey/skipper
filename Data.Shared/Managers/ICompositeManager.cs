using Models.Shared.Entities;
using Models.Shared.Models;

namespace Data.Shared.Managers;

public interface ICompositeManager<TCompositeEntity, TRootEntity, TInfoEntity, TCompositeModel, TRootModel, TInfoModel, TDiscriminator> 
    : IManagerBase<TCompositeEntity, TCompositeModel>
    where TCompositeEntity : class, ICompositeEntity<TRootEntity, TDiscriminator, TInfoEntity>, new()
    where TRootEntity : class, ICompositeEntityRoot<TDiscriminator>, new()
    where TInfoEntity : class, IEntity, new()
    where TCompositeModel : class, ICompositeModel<TRootModel, TDiscriminator, TInfoModel>, new()
    where TRootModel : class, ICompositeModelRoot<TDiscriminator>, new()
    where TInfoModel : class, IModel
{
    
}