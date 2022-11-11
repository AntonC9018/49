using System.Diagnostics;
using fourtynine.Controllers;

namespace fourtynine.Navbar;

public record NavbarAction(
    string Controller,
    string Action,
    string Url,
    string DisplayName)
{
    public static NavbarAction Create(
        LinkGenerator linkGenerator,
        string controllerName,
        string actionName,
        string displayName)
    {
        var url = linkGenerator.GetPathByAction(action: actionName, controller: controllerName);
        Debug.Assert(url is not null, $"Bad action parameters: {controllerName} / {actionName}");

        return new NavbarAction(
            Controller: controllerName,
            Action: actionName,
            Url: url,
            DisplayName: displayName);
    }

    public bool IsSameAction(string controllerName, string actionName)
    {
        // I'm actually not quite sure how it would play out with redirecting actions?
        // We must assert the links never point to redirecting actions.
        return controllerName == Controller && actionName == Action;
    }
}

public interface INavbarActionsService
{
    IEnumerable<NavbarAction> NavbarActions { get; }
    NavbarAction HomeAction { get; }
}

public class NavbarActionsService : INavbarActionsService
{
    public IEnumerable<NavbarAction> NavbarActions => _navbarActions;
    public NavbarAction HomeAction { get; }
    private readonly NavbarAction[] _navbarActions; 
    
    public NavbarActionsService(LinkGenerator linkGenerator)
    {
        HomeAction = NavbarAction.Create(
            linkGenerator,
            nameof(HomeController).ControllerName(),
            nameof(HomeController.Index),
            "Home");

        var privacy = NavbarAction.Create(
            linkGenerator,
            nameof(HomeController).ControllerName(),
            nameof(HomeController.Privacy),
            "Privacy");

        _navbarActions = new[]
        {
            HomeAction,
            privacy,
        };
    }
}