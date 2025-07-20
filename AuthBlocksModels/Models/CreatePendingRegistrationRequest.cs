namespace AuthBlocksModels.Models;

public class CreatePendingRegistrationRequest
{
    public string Email { get; set; } = string.Empty;
    public string ReturnHost { get; set; } = string.Empty;
}