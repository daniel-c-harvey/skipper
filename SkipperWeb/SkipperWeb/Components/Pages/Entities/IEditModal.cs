using Microsoft.AspNetCore.Components;

namespace SkipperWeb.Components.Pages.Entities
{
    public interface IEditModal<TModel> : IComponent
    {
        TModel Model { get; set; }
    }
}
