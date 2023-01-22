using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fourtynine.Pages.Account;

public record struct AuthSchemeInfo(
    string Name,
    string DisplayName);

public class LogIn : PageModel
{
    public IEnumerable<AuthSchemeInfo> Schemes { get; set; } = null!;
    
    [BindProperty(SupportsGet = true)]
    public string? ProviderName { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }
    
    public string PageName => RouteData.GetPage()!;

    private IAuthenticationSchemeProvider GetSchemeProvider() =>
        HttpContext.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

    public async Task<IActionResult> OnGet()
    {
        if (string.IsNullOrWhiteSpace(ProviderName))
        {
            var schemes = await GetSchemeProvider().GetAllSchemesAsync(); 
            Schemes = schemes
                .Where(s => !string.IsNullOrEmpty(s.DisplayName))
                .Select(s => new AuthSchemeInfo(s.Name, s.DisplayName!));
            return Page();
        }

        var scheme = await GetSchemeProvider().GetSchemeAsync(ProviderName);
        if (scheme is null)
            return BadRequest($"The provider {ProviderName} is not supported.");
        
        string redirectUri = ReturnUrl is null || !Url.IsLocalUrl(ReturnUrl)
            ? Url.Page("/Index")!
            : ReturnUrl;
        
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = redirectUri,
        }, scheme.Name);
    }
}