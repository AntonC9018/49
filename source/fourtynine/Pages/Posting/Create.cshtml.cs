using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fourtynine.Postings;

[Authorize]
public class CreatePostingPageModel : PageModel
{
    private readonly PostingApiService _api;
    private readonly ILogger _logger;
    private readonly LinkGenerator _linkGenerator;

    // I need to:
    // - validate the model on the client side
    // - reject the model on the server side with an error message for the client
    // - add the new posting to the database
    // - reroute the client to a different page
    [BindProperty]
    public PostingCreateDto Posting { get; set; } = null!;
    
    public CreatePostingPageModel(
        PostingApiService api,
        ILogger<CreatePostingPageModel> logger, 
        LinkGenerator linkGenerator)
    {
        _api = api;
        _logger = logger;
        _linkGenerator = linkGenerator;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation("Posting with title {Title} is being created", Posting.Title);

        if (!ModelState.IsValid)
            return BadRequest("Posting is not valid");

        var posting = await _api.Create(Posting);
        var url = _linkGenerator.GetPathByPage(HttpContext, "./Index",
            values: new { postingId = posting.Id })!;
        return Content(url);
    }
}
