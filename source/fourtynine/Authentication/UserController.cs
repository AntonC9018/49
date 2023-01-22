using fourtynine.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        [FromQuery] string? scheme,
        [FromServices] IKeyedProvider<IUserEmailConfirmationProvider> emailConfirmationProvider)
    {
        var user = await _userProvider.GetAuthenticatedUser();
        if (user.EmailConfirmed)
            return Ok(true);

        bool newValue = false;
        if (scheme is null)
        {
            var authenticationSchemes = await _userProvider.GetAllowedAuthenticationSchemes();
            var tasks = authenticationSchemes
                .Select(s => emailConfirmationProvider.Get(s.SchemeName))
                .Where(p => p is not null)
                .Select(p => p!.GetEmailConfirmedAsync(user, User))
                .ToList();

            while (tasks.Count > 0)
            {
                var task = await Task.WhenAny(tasks);
                tasks.Remove(task);
                if (task.Result == true)
                {
                    newValue = true;
                    break;
                }
            }
        }
        else
        {
            var provider = emailConfirmationProvider.Get(scheme);
            if (provider is not null)
                newValue = await provider.GetEmailConfirmedAsync(user, User);
        }
        
        if (newValue == true)
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
        [FromServices] ISendGridEmailSender email,
        [FromServices] LinkGenerator linkGenerator)
    {
        var user = await _userProvider.GetAuthenticatedUser();

        if (user.EmailConfirmed)
            return BadRequest("Email already confirmed.");
        
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl = linkGenerator.GetUriByPage(HttpContext, 
            page: "/Account/ConfirmEmail", 
            values: new { userId = user.Id, code = token });

        var message = new SendGridMessage
        {
            Subject = "Email confirmation",
            HtmlContent = $"<p>Please confirm your account by clicking this <a href={callbackUrl}>link</a></p>",
            PlainTextContent = $"Please confirm your account by clicking this link: {callbackUrl}",
        };
        message.AddTo(user.Email);
        var response = await email.SendEmailAsync(message);

        if (response.IsSuccessStatusCode)
            return Ok();
        
        return StatusCode(StatusCodes.Status500InternalServerError, "Failed to send email.");
    }
}