using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace fourtynine.Authentication;

public static class ClaimHelper
{
    public const string AccessTokenType = "access_token";
    
    public static Claim? MaybeAddAccessTokenClaim(this OAuthCreatingTicketContext context)
    {
        var identity = context.Identity;
        if (identity is null)
            return null;
        
        var accessToken = context.AccessToken;
        if (accessToken is null)
            return null;
        
        var claim = new Claim(AccessTokenType, accessToken);
        identity.AddClaim(claim);
        return claim;
    }
    
    public static string? GetAccessToken(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(AccessTokenType);
    }
    
    public static bool IsAccessToken(this Claim claim)
    {
        return claim.Type == AccessTokenType;
    }
}