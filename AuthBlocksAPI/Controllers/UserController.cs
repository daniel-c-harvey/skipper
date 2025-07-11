using System.Linq.Expressions;
using API.Shared.Controllers;
using AuthBlocksData.Services;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthBlocksAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : BaseModelController<ApplicationUser, UserModel>
{
    private readonly IUserService _userService;

    public UserController(IUserService userService) : base(userService)
    {
        _userService = userService;
        
        // Add custom sort expressions
        AddSortExpression(nameof(UserModel.UserName), e => e.UserName ?? string.Empty);
        AddSortExpression(nameof(UserModel.Email), e => e.Email ?? string.Empty);
        AddSortExpression(nameof(UserModel.NormalizedUserName), e => e.NormalizedUserName ?? string.Empty);
        AddSortExpression(nameof(UserModel.NormalizedEmail), e => e.NormalizedEmail ?? string.Empty);
        AddSortExpression(nameof(UserModel.EmailConfirmed), e => e.EmailConfirmed);
        AddSortExpression(nameof(UserModel.PhoneNumber), e => e.PhoneNumber ?? string.Empty);
        AddSortExpression(nameof(UserModel.PhoneNumberConfirmed), e => e.PhoneNumberConfirmed);
        AddSortExpression(nameof(UserModel.TwoFactorEnabled), e => e.TwoFactorEnabled);
        AddSortExpression(nameof(UserModel.LockoutEnabled), e => e.LockoutEnabled);
        AddSortExpression(nameof(UserModel.AccessFailedCount), e => e.AccessFailedCount);
    }

    protected override Expression<Func<ApplicationUser, bool>> BuildSearchPredicate(string? search)
    {
        if (string.IsNullOrEmpty(search))
            return e => true;

        return e => e.UserName!.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                   e.Email!.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                   e.NormalizedUserName!.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                   e.NormalizedEmail!.Contains(search, StringComparison.OrdinalIgnoreCase);
    }
} 