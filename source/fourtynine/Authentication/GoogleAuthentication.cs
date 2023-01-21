using System.Security.Claims;
using fourtynine.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using DbContext = fourtynine.DataAccess.DbContext;

namespace fourtynine.Authentication;

public static class GoogleAuthentication
{
    public static async Task ManageUserInitialization(OAuthCreatingTicketContext context)
    {
        var claimsEnumerable = context.Identity!.Claims;
        var claims = claimsEnumerable as List<Claim> ?? claimsEnumerable.ToList();
        var providerUserId = claims.Find(a => a.Type == ClaimTypes.NameIdentifier)!.Value;

        var serviceProvider = context.HttpContext.RequestServices;
        var dbContext = serviceProvider.GetRequiredService<DbContext>();
        var matchedUsersQuery = dbContext.Users
            .Where(u => u.AllowedAuthenticationSchemes.Any(
                s => s.SchemeName == context.Scheme.Name
                     && s.ProviderUserId == providerUserId))
            .Include(u => u.AllowedAuthenticationSchemes);
        var matchedUsers = await matchedUsersQuery.ToListAsync();

        if (matchedUsers.Count > 1)
            throw new Exception("Multiple users matched the same GitHub account?");

        var userName = claims.Find(a => a.Type == ClaimTypes.Name)!.Value;
        var email = claims.Find(a => a.Type == ClaimTypes.Email)!.Value;
        var tokenClaim = context.MaybeAddAccessTokenClaim();
        ApplicationUser user;
        
        if (matchedUsers.Count == 1)
        {
            // Would you like to update the email or the user name?
            user = matchedUsers[0];
        }
        else
        {
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
    
                // There doesn't seem to be a way to check if the user has verified their email address.
                EmailConfirmed = false,
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        
        var authStore = serviceProvider.GetRequiredService<IApplicationUserStore>();
        authStore.User = user;
    }
    
    public static AuthenticationBuilder AddGoogleAuthentication(
        this AuthenticationBuilder authBuilder, IConfiguration configuration)
    {
        return authBuilder.AddGoogle(options =>
        {
            options.SignInScheme = AuthTokenSources.AnySchemeName;
            options.CallbackPath = "/account/authorize/Google";
            options.ClientId = configuration["OAuthGoogleClientId"];
            options.ClientSecret = configuration["OAuthGoogleClientSecret"];

            options.SaveTokens = true;
            options.Events.OnCreatingTicket += ManageUserInitialization;
        });
    }
}