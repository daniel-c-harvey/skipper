using Models.Shared.Entities;
using Models.Shared.Models;
using AuthBlocksModels.Entities.Identity;

namespace AuthBlocksModels.Models;

public class RoleModel : IModel<RoleModel, ApplicationRole>
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string? ConcurrencyStamp { get; set; }
    public long? ParentRoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public static ApplicationRole CreateEntity(RoleModel model)
    {
        return new ApplicationRole
        {
            Id = model.Id,
            Name = model.Name,
            NormalizedName = model.NormalizedName,
            ConcurrencyStamp = model.ConcurrencyStamp,
            ParentRoleId = model.ParentRoleId,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }
} 