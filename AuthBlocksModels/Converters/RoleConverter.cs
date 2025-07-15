using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.InputModels;
using AuthBlocksModels.Models;
using Models.Shared.Converters;

namespace AuthBlocksModels.Converters;

public class RoleEntityToModelConverter : IEntityToModelConverter<ApplicationRole, RoleModel>
{
    public static RoleModel Convert(ApplicationRole entity)
    {
        return new RoleModel
        {
            Id = entity.Id,
            Name = entity.Name ?? string.Empty,
            NormalizedName = entity.NormalizedName ?? string.Empty,
            ConcurrencyStamp = entity.ConcurrencyStamp,
            ParentRoleId = entity.ParentRoleId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }

    public static ApplicationRole Convert(RoleModel model)
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

public class RoleModelToInputConverter : IModelToInputConverter<RoleModel, RoleInputModel>
{
    public static RoleInputModel Convert(RoleModel model)
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

    public static RoleModel Convert(RoleInputModel input)
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
}