using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData;

namespace fourtynine.Postings;

public class PostingSearchPageModel : PageModel
{
    private readonly PostingApiService _api;

    [BindProperty(SupportsGet = true)]
    public ODataQueryOptions<PostingGetDto_Detailed> Query { get; private set; } = null!;
    
    public ICollection<PostingGetDto_Detailed>? Results { get; set; }
    public ODataException? Exception { get; set; }
    
    public PostingSearchPageModel(PostingApiService api)
    {
        _api = api;
    }
    
    public async Task OnGet()
    {
        try
        {
            Results = await _api.GetODataQuery(Query);
        }
        catch (ODataException exc)
        {
            Exception = exc;
        }
        Page();
    }
}