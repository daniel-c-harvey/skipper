﻿using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.InputModels;
using AuthBlocksModels.Models;
using Models.Shared.Converters;

namespace AuthBlocksModels.Converters;

public class UserEntityToModelConverter : IEntityToModelConverter<ApplicationUser, UserModel>
{
    public static UserModel Convert(ApplicationUser entity)
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

    public static ApplicationUser Convert(UserModel model)
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

public class UserModelToInputConverter : IModelToInputConverter<UserModel, UserInputModel>
{
    public static UserInputModel Convert(UserModel model)
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

    public static UserModel Convert(UserInputModel input)
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
}