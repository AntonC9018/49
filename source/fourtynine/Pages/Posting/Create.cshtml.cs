using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fourtynine.Postings;

public class CreatePostingPageModel : PageModel
{
    [BindProperty]
    public PostingCreateDto Data { get; set; }
}