﻿using AuthBlocksModels.Models;
using NetBlocks.Models;

namespace AuthBlocksAPI.Models;

public class TokenValidationResult : ResultBase<TokenValidationResult>
{
    public long? PendingRegistrationId { get; set; }
    public bool IsConsumed { get; set; }
    public IEnumerable<RoleModel>? Roles { get; set; }
    
    public TokenValidationResult() : base()
    {
        PendingRegistrationId = null;
        IsConsumed = false;       
    }

    public TokenValidationResult(long pendingRegistrationId, bool isConsumed, IEnumerable<RoleModel>? roles) : base()
    {
        PendingRegistrationId = pendingRegistrationId;
        IsConsumed = isConsumed;
        Roles = roles;
    }
}