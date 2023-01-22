using System.Security.Claims;
using fourtynine.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace fourtynine.Authentication;

public class GithubEmailConfirmationProvider : IUserEmailConfirmationProvider
{
    public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, ClaimsPrincipal principal)
    {
        return Task.FromResult(false);
    }
}

public static class GithubAuthentication
{
    public static AuthenticationBuilder AddGithubAuthentication(
        this AuthenticationBuilder authBuilder, IConfiguration configuration)
    {
        return authBuilder.AddGitHub(options =>
        {
            options.SignInScheme = AuthTokenSources.AnySchemeName;
            options.CallbackPath = "/account/authorize/GitHub";
            options.ClientId = configuration["OAuthGithubClientId"];
            options.ClientSecret = configuration["OAuthGithubClientSecret"];

            options.SaveTokens = true;
            options.Scope.Add("read:user");
            options.Events.OnCreatingTicket += AuthenticationSomething.ManageUserInitialization;
        });
    }
}