using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fourtynine.Postings;

public class CreatePostingPageModel : PageModel
{
    [BindProperty]
    public PostingCreateDto Model { get; set; }
}

public class PostingController : Controller
{
    public IActionResult Create()
    {
        return View();
    }
}