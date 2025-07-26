using Models.Shared.Entities;
using Models.Shared.Models;

namespace Models.Shared.Converters;

public interface ICompositeEntityToModelConverter<TCompositeEntity, TRootEntity, TInfo, TCompositeModel, TRootModel, TInfoModel, TDiscriminator> 
    : IConverter<TCompositeEntity, TCompositeModel>
    where TCompositeEntity : class, ICompositeEntity<TRootEntity, TDiscriminator, TInfo>
    where TRootEntity : class, ICompositeEntityRoot<TDiscriminator>, new()
    where TInfo : class, IEntity, new()
    where TCompositeModel : class, ICompositeModel<TRootModel, TDiscriminator, TInfoModel>, new()
    where TRootModel : class, ICompositeModelRoot<TDiscriminator>, new()
    where TInfoModel : class, IModel
{
}