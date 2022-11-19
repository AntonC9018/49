using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fourtynine.Postings;

public class CreatePostingPageModel : PageModel
{
    private readonly PostingApiService _api;
    private readonly ILogger _logger;
    
    public CreatePostingPageModel(PostingApiService api, ILogger<CreatePostingPageModel> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task OnPostAsync(PostingCreateDto Posting)
    {
        _logger.LogInformation("Posting with title {title} is being created", Posting.Title);

        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Posting is not valid");
            return;
        }

        await _api.Create(Posting);
    }
}