using System.Security.Claims;
using fourtynine.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace fourtynine.Authentication;

public class GoogleEmailConfirmationProvider : IUserEmailConfirmationProvider
{
    public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, ClaimsPrincipal principal)
    {
        return Task.FromResult(true);
    }
}

public static class GoogleAuthentication
{
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
            options.Events.OnCreatingTicket += AuthenticationSomething.ManageUserInitialization;
        });
    }
}