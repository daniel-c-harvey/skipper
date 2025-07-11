using Models.Shared.Entities;
using Models.Shared.Models;
using AuthBlocksModels.Entities.Identity;

namespace AuthBlocksModels.Models;

public class UserModel : IModel<UserModel, ApplicationUser>
{
    public long Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NormalizedUserName { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; }
    public int AccessFailedCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public static ApplicationUser CreateEntity(UserModel model)
    {
        return new ApplicationUser
        {
            Id = model.Id,
            UserName = model.UserName,
            Email = model.Email,
            NormalizedUserName = model.NormalizedUserName,
            NormalizedEmail = model.NormalizedEmail,
            EmailConfirmed = model.EmailConfirmed,
            PhoneNumber = model.PhoneNumber,
            PhoneNumberConfirmed = model.PhoneNumberConfirmed,
            TwoFactorEnabled = model.TwoFactorEnabled,
            LockoutEnd = model.LockoutEnd,
            LockoutEnabled = model.LockoutEnabled,
            AccessFailedCount = model.AccessFailedCount,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }
} 