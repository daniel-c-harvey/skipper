using Models.Shared.Entities;
using Models.Shared.Models;
using Models.Shared.InputModels;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;

namespace AuthBlocksModels.InputModels;

public class RoleInputModel : IInputModel<RoleInputModel, RoleModel, ApplicationRole>
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string? ConcurrencyStamp { get; set; }
    public long? ParentRoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    
    public static RoleModel MakeModel(RoleInputModel input)
    {
        return new RoleModel
        {
            Id = input.Id,
            Name = input.Name,
            NormalizedName = input.NormalizedName,
            ConcurrencyStamp = input.ConcurrencyStamp,
            CreatedAt = input.CreatedAt,
            UpdatedAt = input.UpdatedAt,
            ParentRoleId = input.ParentRoleId
        };
    }

    public static RoleInputModel From(RoleModel model)
    {
        return new RoleInputModel
        {
            Id = model.Id,
            Name = model.Name,
            NormalizedName = model.NormalizedName,
            ConcurrencyStamp = model.ConcurrencyStamp,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
            ParentRoleId = model.ParentRoleId
        };
    }
} 