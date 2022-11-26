using System.Diagnostics;

namespace fourtynine.Navbar;

public record struct RouteInfo(string ControllerOrPage, string? Action);

public interface INavbarAction
{
    string DisplayName { get; }
    string Path { get; }
    bool IsMatch(in RouteInfo routeInfo);
}

public record NavbarControllerAction(
    string Controller,
    string? Action,
    string Path,
    string DisplayName) : INavbarAction
{
    public static NavbarControllerAction Create(
        LinkGenerator linkGenerator,
        string controllerName,
        string actionName,
        string displayName)
    {
        var path = linkGenerator.GetPathByAction(action: actionName, controller: controllerName);
        Debug.Assert(path is not null, $"Bad action parameters: {controllerName} / {actionName}");

        return new NavbarControllerAction(
            Controller: controllerName,
            Action: actionName,
            Path: path,
            DisplayName: displayName);
    }

    public bool IsMatch(in RouteInfo routeInfo)
    {
        // I'm actually not quite sure how it would play out with redirecting actions?
        // We must assert the links never point to redirecting actions.
        return routeInfo.ControllerOrPage == Controller
            && routeInfo.Action == Action;
    }
}

public record NavbarPageAction(
    string Page,
    string Path,
    string DisplayName) : INavbarAction
{
    public static NavbarPageAction Create(
        LinkGenerator linkGenerator,
        string page,
        string displayName)
    {
        var path = linkGenerator.GetPathByPage(page);
        Debug.Assert(path is not null, $"Bad page parameters: {page}");

        return new NavbarPageAction(
            Page: page,
            Path: path,
            DisplayName: displayName);
    }

    public bool IsMatch(in RouteInfo routeInfo)
    {
        return routeInfo.ControllerOrPage == Page && routeInfo.Action is null;
    }
}

public interface INavbarActionsService
{
    IEnumerable<INavbarAction> NavbarActions { get; }
    INavbarAction HomeAction { get; }
    INavbarAction Login { get; }
    INavbarAction Logout { get; }
}

public class NavbarActionsService : INavbarActionsService
{
    public IEnumerable<INavbarAction> NavbarActions => _navbarActions;
    public INavbarAction HomeAction { get; }
    public INavbarAction Login { get; }
    public INavbarAction Logout { get; }
    private readonly INavbarAction[] _navbarActions; 
    
    public NavbarActionsService(LinkGenerator linkGenerator)
    {
        HomeAction = NavbarPageAction.Create(
            linkGenerator,
            page: "/Index",
            "Home");
        
        var create = NavbarPageAction.Create(
            linkGenerator,
            page: "/Posting/Create",
            "New Posting");
        
        var privacy = NavbarPageAction.Create(
            linkGenerator,
            page: "/Privacy",
            "Privacy");
        
        Login = NavbarPageAction.Create(
            linkGenerator,
            page: "/Account/Login",
            "Login");
        
        Logout = NavbarPageAction.Create(
            linkGenerator,
            page: "/Account/Logout",
            "Logout");
        

        _navbarActions = new INavbarAction[]
        {
            create,
            privacy,
        };
    }
}