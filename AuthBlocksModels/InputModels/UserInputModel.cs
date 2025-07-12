using Models.Shared.Entities;
using Models.Shared.Models;
using Models.Shared.InputModels;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;

namespace AuthBlocksModels.InputModels;

public class UserInputModel : IInputModel<UserInputModel, UserModel, ApplicationUser>
{
    public long Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool LockoutEnabled { get; set; }
    public int AccessFailedCount { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public static UserModel MakeModel(UserInputModel input)
    {
        return new UserModel
        {
            Id = input.Id,
            UserName = input.UserName,
            Email = input.Email,
            PhoneNumber = input.PhoneNumber,
            EmailConfirmed = input.EmailConfirmed,
            PhoneNumberConfirmed = input.PhoneNumberConfirmed,
            TwoFactorEnabled = input.TwoFactorEnabled,
            LockoutEnabled = input.LockoutEnabled,
            AccessFailedCount = input.AccessFailedCount,
            UpdatedAt = input.UpdatedAt,
            CreatedAt = input.CreatedAt
        };
    }

    public static UserInputModel From(UserModel model)
    {
        return new UserInputModel
        {
            Id = model.Id,
            UserName = model.UserName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            EmailConfirmed = model.EmailConfirmed,
            PhoneNumberConfirmed = model.PhoneNumberConfirmed,
            TwoFactorEnabled = model.TwoFactorEnabled,
            LockoutEnabled = model.LockoutEnabled,
            AccessFailedCount = model.AccessFailedCount,
            UpdatedAt = model.UpdatedAt,
            CreatedAt = model.CreatedAt
        };
    }
}
