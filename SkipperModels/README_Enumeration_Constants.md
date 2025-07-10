# Enumeration Constants Generator

This source generator automatically creates compile-time constants for any class that inherits from `Enumeration<T>` and is marked with the `[GenerateConstants]` attribute.

## Usage

### 1. Mark Your Enumeration Class

Add the `[GenerateConstants]` attribute to any class that inherits from `Enumeration<T>`:

```csharp
using NetBlocks.Utilities;
using SkipperModels.Attributes;

namespace AuthBlocksModels.SystemDefinitions;

[GenerateConstants]
public class SystemRole : Enumeration<SystemRole>
{
    public static SystemRole UserAdmin = new(2, "UserAdmin");
    public static SystemRole Admin = new(1, "Admin", [UserAdmin]);
    // ...
}
```

### 2. Use Generated Constants

The source generator will automatically create a `Constants` class:

```csharp
// Generated automatically
public static class SystemRoleConstants
{
    public const string UserAdmin = "UserAdmin";
    public const string Admin = "Admin";
}
```

### 3. Use in Attributes

Now you can use these constants in attributes:

```csharp
[Authorize(Roles = SystemRoleConstants.Admin)]
public class AdminController : ControllerBase
{
    [HttpGet("admin-only")]
    [Authorize(Roles = SystemRoleConstants.Admin)]
    public IActionResult AdminOnly()
    {
        return Ok("Admin access granted");
    }
}
```

## How It Works

1. The source generator scans for classes that inherit from `Enumeration<T>`
2. It looks for the `[GenerateConstants]` attribute
3. It extracts all static fields from the class
4. It generates compile-time constants for each static field name

## Benefits

- **Compile-time Safety**: Constants are resolved at compile time
- **Type Safety**: IntelliSense and compile-time checking
- **Refactoring Support**: Renaming fields automatically updates all references
- **No Runtime Overhead**: Constants are inlined at compile time

## Generated Files

The source generator creates files with the `.g.cs` extension in the `obj` folder:
- `SystemRoleConstants.g.cs`

These files are automatically included in your compilation and are regenerated whenever you build the project.
