using fourtynine.Authentication;
using fourtynine.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DbContext = fourtynine.DataAccess.DbContext;

namespace fourtynine.Pages.Account;

public class ConfirmEmail : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public ConfirmEmail(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public IdentityResult EmailValidationResult { get; private set; } = default!;
    public ApplicationUser ApplicationUser { get; private set; } = default!;
        

    public async Task<IActionResult> OnGet(
        [FromQuery] string? code, [FromQuery] string? userId)
    {
        if (code is null || userId is null)
            return NotFound();
        
        if (!Guid.TryParse(userId, out var providedGuid))
            return BadRequest("Invalid user id");
        
        var id = HttpContext.GetUserId();
        
        // Is this check required?
        if (id != providedGuid)
        {
            // return BadRequest("The user id has to match");
        }

        // It takes the user id as a string? this is bad design imo.
        // It should be a generic extension method and take the right type as the id.
        ApplicationUser = await _userManager.FindByIdAsync(userId);
        EmailValidationResult = await _userManager.ConfirmEmailAsync(ApplicationUser, code);
        
        return Page();
    }
}