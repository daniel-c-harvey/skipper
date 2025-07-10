using Microsoft.AspNetCore.Components;

namespace SkipperWeb.Components.Pages.Maintenance.Entities
{
    public interface IEditModal<TModel> : IComponent
    {
        TModel Model { get; set; }
    }
}
