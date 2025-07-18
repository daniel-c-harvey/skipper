using NetBlocks.Models;

namespace AuthBlocksModels.ApiModels;

public class TokenCreationResult : ResultBase<TokenCreationResult>
{
    public string RegistrationEmail { get; set; }
    public string? RegistrationToken { get; set; }
    
    public TokenCreationResult() : base()
    {
        RegistrationToken = null;
        RegistrationEmail = string.Empty;
    }
    
    public TokenCreationResult(string registrationEmail, string registrationToken) : base()
    {
        RegistrationEmail = registrationEmail;
        RegistrationToken = registrationToken;
    }

    public static TokenCreationResult CreatePassResult(string registrationEmail, string registrationToken)
    {
        var result = ResultBase<TokenCreationResult>.CreatePassResult();
        result.RegistrationEmail = registrationEmail;
        result.RegistrationToken = registrationToken;
        return result;
    }
}

public class TokenCreationResultDto : ResultBase<TokenCreationResult>.ResultDtoBase<TokenCreationResult, TokenCreationResultDto>
{
    public string RegistrationEmail { get; set; }
    public string? RegistrationToken { get; set; }

    public TokenCreationResultDto() : base()
    {
        RegistrationEmail = string.Empty;
        RegistrationToken = null;
    }
    
    public TokenCreationResultDto(string registrationEmail, string registrationToken) : base()
    {
        RegistrationEmail = registrationEmail;
        RegistrationToken = registrationToken;
    }
    
    public TokenCreationResultDto(TokenCreationResult result) : base(result)
    {
        RegistrationEmail = result.RegistrationEmail;
        RegistrationToken = result.RegistrationToken;
    }
    public override TokenCreationResult From()
    {
        var result = base.From();
        result.RegistrationEmail = RegistrationEmail;
        result.RegistrationToken = RegistrationToken;
        return result;
    }
}