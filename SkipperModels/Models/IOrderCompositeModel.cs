using Models.Shared.Models;

using SkipperModels.Models;

namespace SkipperModels.Composites;

public interface IOrderCompositeModel<TInfoModel> : ICompositeModel<OrderModel, OrderType, TInfoModel>
where TInfoModel : IModel
{
    
}