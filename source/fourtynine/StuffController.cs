using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace fourtynine;

[ApiControllerConvention]
public class StuffController : Controller
{
    private LinkGenerator _linkGenerator;

    public StuffController(LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }

    [HttpGet]
    public IActionResult GetStuff()
    {
        return Ok(new
        {
            Name = "Frank"
        });
    }
}