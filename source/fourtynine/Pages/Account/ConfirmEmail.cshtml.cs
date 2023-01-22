using fourtynine.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SendGrid.Helpers.Mail;

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
        [FromQuery] string? code,
        [FromQuery] string? userId,
        [FromServices] ISendGridEmailSender emailSender)
    {
        if (code is null || userId is null)
            return NotFound();
        
        if (!Guid.TryParse(userId, out var providedGuid))
            return BadRequest("Invalid user id");
        
        // Is this check required?
        #if false
        var loggedInUserId = User.GetId();
        if (loggedInUserId != providedGuid)
        {
            return BadRequest("The user id has to match");
        }
        #endif

        // It takes the user id as a string? this is bad design imo.
        // It should be a generic extension method and take the right type as the id.
        ApplicationUser = await _userManager.FindByIdAsync(userId);
        if (ApplicationUser.EmailConfirmed)
            return Page();
        
        EmailValidationResult = await _userManager.ConfirmEmailAsync(ApplicationUser, code);

        if (EmailValidationResult.Succeeded)
        {
            var message = new SendGridMessage
            {
                PlainTextContent = "Your email has been confirmed",
                Subject = "Email confirmed",
            };
            message.AddTo(ApplicationUser.Email);
            await emailSender.SendEmailAsync(message);
        }
        
        return Page();
    }
}