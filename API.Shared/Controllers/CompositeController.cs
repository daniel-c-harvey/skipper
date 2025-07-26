using Data.Shared.Managers;
using Models.Shared.Entities;
using Models.Shared.Models;

namespace API.Shared.Controllers;

public class CompositeController<TCompositeEntity, TRootEntity, TInfoEntity, TCompositeModel, TRootModel, TInfoModel, TDiscriminator, TManager> 
    : ModelControllerBase<TCompositeEntity, TCompositeModel, TManager>, 
        ICompositeController<TCompositeModel, TRootModel, TDiscriminator, TInfoModel>
    where TCompositeEntity : class, ICompositeEntity<TRootEntity, TDiscriminator, TInfoEntity>, new()
    where TRootEntity : class, ICompositeEntityRoot<TDiscriminator>, new()
    where TInfoEntity : class, IEntity, new()
    where TCompositeModel : class, ICompositeModel<TRootModel, TDiscriminator, TInfoModel>, new()
    where TRootModel : class, ICompositeModelRoot<TDiscriminator>, new()
    where TInfoModel : class, IModel
    where TManager : ICompositeManager<TCompositeEntity, TRootEntity, TInfoEntity, TCompositeModel, TRootModel, TInfoModel, TDiscriminator>
{
    public CompositeController(TManager manager) : base(manager)
    {
    }
}