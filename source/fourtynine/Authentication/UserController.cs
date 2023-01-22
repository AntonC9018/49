using fourtynine.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Mail;
using DbContext = fourtynine.DataAccess.DbContext;

namespace fourtynine.Authentication;

[ApiRoute]
[ApiControllerConvention]
[Authorize]
public class UserController : Controller
{
    private readonly DbContext _dbContext;
    private readonly UserProviderService _userProvider;

    public UserController(DbContext dbContext, UserProviderService userProvider)
    {
        _dbContext = dbContext;
        _userProvider = userProvider;
    }

    [HttpPatch("email/refresh-validity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> RefreshEmailValidity(
        [FromServices] IUserEmailConfirmationProvider emailConfirmationProvider)
    {
        var user = await _userProvider.GetAuthenticatedUser();
        bool newValue = await emailConfirmationProvider.GetEmailConfirmedAsync(user, HttpContext.User);
        if (user.EmailConfirmed != newValue)
        {
            user.EmailConfirmed = newValue;
            await _dbContext.SaveChangesAsync();
        }

        return Ok(newValue);
    }
    
    [HttpPost("email/send-validation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendEmailValidationMessage(
        [FromServices] UserManager<ApplicationUser> userManager,
        [FromServices] ISendGridEmailSender email)
    {
        var user = await _userProvider.GetAuthenticatedUser();

        if (user.EmailConfirmed)
            return BadRequest("Email already confirmed.");
        
        var token = userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl = Url.Page("Account/ConfirmEmail", 
            new { userId = user.Id, code = token });

        var message = new SendGridMessage
        {
            Subject = "Email confirmation",
            PlainTextContent = @"<p>Please confirm your account by clicking this link: <a href=" + callbackUrl +
                               ">link</a></p>",
        };
        message.AddTo(user.Email);
        var response = await email.SendEmailAsync(message);

        if (response.IsSuccessStatusCode)
            return Ok();
        
        return StatusCode(StatusCodes.Status500InternalServerError, "Failed to send email.");
    }
}