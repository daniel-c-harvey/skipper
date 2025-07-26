using Microsoft.AspNetCore.Mvc;
using Models.Shared.Common;
using Models.Shared.Entities;
using Models.Shared.Models;
using NetBlocks.Models;

namespace API.Shared.Controllers;

public interface IModelController<TModel> : IModelControllerBase<TModel>
where TModel : class, IModel, new()
{
    
}