using Data.Shared.Data.Repositories;
using Models.Shared.Converters;
using Models.Shared.Entities;
using Models.Shared.Models;

namespace Data.Shared.Managers;

public abstract class CompositeManager<TCompositeEntity, TRootEntity, TInfoEntity, TCompositeModel, TRootModel, TInfoModel,  TDiscriminator, TRepository, TConverter> 
    : ManagerBase<TCompositeEntity, TCompositeModel, TRepository, TConverter>, 
        ICompositeManager<TCompositeEntity, TRootEntity, TInfoEntity, TCompositeModel, TRootModel, TInfoModel, TDiscriminator>
    where TCompositeEntity : class, ICompositeEntity<TRootEntity,TDiscriminator,TInfoEntity>, new()
    where TRootEntity : class, ICompositeEntityRoot<TDiscriminator>, new()
    where TInfoEntity : class, IEntity, new()
    where TCompositeModel : class, ICompositeModel<TRootModel, TDiscriminator, TInfoModel>, new()
    where TRootModel : class, ICompositeModelRoot<TDiscriminator>, new()
    where TInfoModel : class, IModel, new()
    where TRepository : ICompositeRepository<TCompositeEntity, TRootEntity, TDiscriminator, TInfoEntity>
    where TConverter : ICompositeEntityToModelConverter<TCompositeEntity, TRootEntity, TInfoEntity, TCompositeModel, TRootModel, TInfoModel, TDiscriminator>
{
    protected CompositeManager(TRepository repository) : base(repository)
    {
    }
}