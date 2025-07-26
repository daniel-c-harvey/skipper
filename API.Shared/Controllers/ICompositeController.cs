using Models.Shared.Models;

namespace API.Shared.Controllers;

public interface ICompositeController<TCompositeModel, TRootModel, TDiscriminator, TInfoModel> : IModelControllerBase<TCompositeModel>
where TCompositeModel : class, ICompositeModel<TRootModel, TDiscriminator, TInfoModel>, new()
where TRootModel : class, ICompositeModelRoot<TDiscriminator>, new()
where TInfoModel : class, IModel
{
    
}