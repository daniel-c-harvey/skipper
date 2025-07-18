using NetBlocks.Models;

namespace AuthBlocksAPI.Models;

public class TokenValidationResult : ResultBase<TokenValidationResult>
{
    public long? PendingRegistrationId { get; set; }
    public bool IsConsumed { get; set; }
    
    public TokenValidationResult() : base()
    {
        PendingRegistrationId = null;
        IsConsumed = false;       
    }

    public TokenValidationResult(long pendingRegistrationId, bool isConsumed) : base()
    {
        PendingRegistrationId = pendingRegistrationId;
        IsConsumed = isConsumed;
    }
}