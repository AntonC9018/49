using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fourtynine;

using static RoutingHelper;

[ApiRoute]
[ApiControllerConvention]
[ApiExplorerSettings(IgnoreApi = true)]
public class StuffController : Controller
{
    private readonly LinkGenerator _linkGenerator;
    private readonly ILogger _logger;

    public StuffController(LinkGenerator linkGenerator, ILogger<StuffController> logger)
    {
        _linkGenerator = linkGenerator;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetStuff()
    {
        _logger.LogInformation("Frank is here");
        return Ok(new
        {
            Name = "Frank",
            Path = _linkGenerator.GetPathByAction(
                nameof(StuffController.GetStuff),
                nameof(StuffController).ControllerName())
        });
    }
    
    [HttpPost]
    [Authorize]
    public IActionResult PostStuff()
    {
        _logger.LogInformation("Restricted resource");
        return Ok("Restricted resource");
    }
}