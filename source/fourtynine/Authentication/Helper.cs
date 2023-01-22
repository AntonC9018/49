using System.Security.Claims;
using fourtynine.DataAccess;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using DbContext = fourtynine.DataAccess.DbContext;

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

public interface IUserEmailConfirmationProvider
{
    // This requiring the whole entity is also a problem...
    Task<bool> GetEmailConfirmedAsync(ApplicationUser user, ClaimsPrincipal principal);
}

public static class AuthenticationSomething
{
    public static async Task ManageUserInitialization(OAuthCreatingTicketContext context)
    {
        var tokenClaim = context.MaybeAddAccessTokenClaim();

        var serviceProvider = context.HttpContext.RequestServices;
        var dbContext = serviceProvider.GetRequiredService<DbContext>();
        var emailConfirmation = serviceProvider.GetKeyedService<IUserEmailConfirmationProvider>(context.Scheme.Name);
        
        var claimsEnumerable = context.Identity!.Claims;
        var claims = claimsEnumerable as List<Claim> ?? claimsEnumerable.ToList();
        var providerUserIdClaim = claims.Find(a => a.Type == ClaimTypes.NameIdentifier)!;
        var providerUserId = providerUserIdClaim.Value;
        ApplicationUser user;

        var matchedUsersQuery = GetUserQuery(dbContext, context.Scheme.Name, providerUserId); 
        var matchedUsers = await matchedUsersQuery.ToListAsync();

        if (matchedUsers.Count > 1)
            throw new Exception("Multiple users matched the same account?");

        
        if (matchedUsers.Count == 1)
        {
            // Would you like to update the email or the user name?
            user = matchedUsers[0];
        }
        else
        {
            var userName = claims.Find(a => a.Type == ClaimTypes.Name)!.Value;
            var email = claims.Find(a => a.Type == ClaimTypes.Email)!.Value;
            
            user = new ApplicationUser
            {
                Email = email,
                UserName = userName,

                // Allow logging in only with github
                AllowedAuthenticationSchemes = new List<AllowedAuthenticationScheme>
                {
                    new()
                    {
                        SchemeName = context.Scheme.Name,
                        ProviderUserId = providerUserId,
                    },
                },
            };
            if (emailConfirmation is not null)
                user.EmailConfirmed = await emailConfirmation.GetEmailConfirmedAsync(user, context.Principal!);

            if (user.Id == default)
            {
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }
        }
        
        context.Identity.RemoveClaim(providerUserIdClaim);
        context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
    }

    public static IQueryable<ApplicationUser> GetUserQuery(
        DbContext dbContext, string schemeName, string providerUserId)
    {
        var matchedUsersQuery = dbContext.Users
            .Where(u => u.AllowedAuthenticationSchemes.Any(
                s => s.SchemeName == schemeName
                     && s.ProviderUserId == providerUserId))
            .Include(u => u.AllowedAuthenticationSchemes);

        return matchedUsersQuery;
    }
}