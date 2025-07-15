using Models.Shared.InputModels;

namespace AuthBlocksModels.InputModels;

public class RoleInputModel : IInputModel
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string? ConcurrencyStamp { get; set; }
    public long? ParentRoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
} 