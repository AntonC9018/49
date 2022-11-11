using Microsoft.AspNetCore.Mvc;

namespace fourtynine.Navbar;

[ApiRoute]
[ApiControllerConvention]
public class NavbarActionsController : Controller
{
    private readonly INavbarActionsService _navbarActions;

    public NavbarActionsController(INavbarActionsService navbarActions)
    {
        _navbarActions = navbarActions;
    }

    [HttpGet]
    public IEnumerable<NavbarAction> GetActions()
    {
        return _navbarActions.NavbarActions;
    }
}