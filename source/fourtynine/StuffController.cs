using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace fourtynine;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public abstract class ApiControllerBase : Controller
{
}

public class StuffController : ApiControllerBase
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