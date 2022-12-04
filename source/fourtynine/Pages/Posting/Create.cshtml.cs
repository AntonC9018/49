using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fourtynine.Postings;

[Authorize]
public class CreatePostingPageModel : PageModel
{
    [BindProperty]
    public PostingCreateDto Posting { get; set; } = null!;
}
