using Microsoft.AspNetCore.Identity;
using Models.Shared.Entities;
using Models.Shared.Models;
using AuthBlocksModels.Models;

namespace AuthBlocksModels.Entities.Identity;

public class ApplicationUser : IdentityUser<long>, IEntity<ApplicationUser, UserModel>
{
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Legacy properties for backward compatibility
    public bool Deleted 
    { 
        get => IsDeleted; 
        set => IsDeleted = value; 
    }
    
    public DateTime Created 
    { 
        get => CreatedAt; 
        set => CreatedAt = value; 
    }
    
    public DateTime Modified 
    { 
        get => UpdatedAt; 
        set => UpdatedAt = value; 
    }
    
    public static UserModel CreateModel(ApplicationUser entity)
    {
        return new UserModel
        {
            Id = entity.Id,
            UserName = entity.UserName ?? string.Empty,
            Email = entity.Email ?? string.Empty,
            NormalizedUserName = entity.NormalizedUserName ?? string.Empty,
            NormalizedEmail = entity.NormalizedEmail ?? string.Empty,
            EmailConfirmed = entity.EmailConfirmed,
            PhoneNumber = entity.PhoneNumber ?? string.Empty,
            PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
            TwoFactorEnabled = entity.TwoFactorEnabled,
            LockoutEnd = entity.LockoutEnd,
            LockoutEnabled = entity.LockoutEnabled,
            AccessFailedCount = entity.AccessFailedCount,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}