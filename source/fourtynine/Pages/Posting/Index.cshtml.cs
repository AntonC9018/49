using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fourtynine.Postings;

public class PostingIndexPageModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public long? PostingId { get; set; }
    
    // Initialized in OnGet
    public PostingGetDto_Detailed? Posting { get; set; }
    public string NewUrl { get; set; } = null!;

    private PostingApiService _api;
    private LinkGenerator _linkGenerator;
    
    public PostingIndexPageModel(
        PostingApiService api, 
        LinkGenerator linkGenerator)
    {
        _api = api;
        _linkGenerator = linkGenerator;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        if (PostingId is null)
            return RedirectToPage("./Search");
        
        var posting = await _api.GetDetailed(PostingId.Value);
        if (posting is null)
            return Page();
        
        Posting = posting;

        var slug = posting.General.Slug;
        NewUrl = _linkGenerator.GetPathByPage(HttpContext,
            values: new { postingId = PostingId, slug })!;

        return Page();
    }
}