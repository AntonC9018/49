using System.Diagnostics;
using fourtynine.Navbar;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace fourtynine;

public static class RoutingHelper
{
    public static string ControllerName(this string nameMaybeWithControllerPart)
    {
        string result = nameMaybeWithControllerPart;
        if (result.EndsWith("Controller"))
            result = result[.. ^"Controller".Length];
        return result;
    }
    public static string ControllerName(this System.Type controllerType)
    {
        return ControllerName(controllerType.Name);
    }

    public static ReadOnlySpan<char> ControllerNameSpan(this ReadOnlySpan<char> nameMaybeWithControllerPart)
    {
        ReadOnlySpan<char> result = nameMaybeWithControllerPart;
        if (result.EndsWith("Controller"))
            result = result[.. ^"Controller".Length];
        return result;
    }
    public static ReadOnlySpan<char> ControllerNameSpan(this System.Type controllerType)
    {
        return ControllerNameSpan(controllerType.Name);
    }

    public static ReadOnlySpan<char> GetCurrentControllerNameSpan(this ViewContext view)
    {
        string? name = (string?) view.RouteData.Values["controller"];
        Debug.Assert(name is not null, "Controller in route must not be null");
        return ControllerNameSpan(name);
    }
    
    public static string GetCurrentControllerName(this ViewContext view)
    {
        string? name = (string?) view.RouteData.Values["controller"];
        Debug.Assert(name is not null, "Controller in route must not be null");
        return ControllerName(name);
    }

    public static string GetCurrentActionName(this ViewContext view)
    {
        string? name = (string?) view.RouteData.Values["action"];
        Debug.Assert(name is not null, "Action in route must not be null");
        return name;
    }

    public static RouteInfo GetCurrentRouteInfo(this ViewContext view)
    {
        string? controller = (string?) view.RouteData.Values["controller"];
        if (controller is null)
        {
            string? page = (string?) view.RouteData.Values["page"];
            Debug.Assert(page is not null, "Neither a controller nor a page?");
            return new RouteInfo(page, null);
        }

        return new RouteInfo(controller, GetCurrentActionName(view));
    }

    public static string? GetPage(this RouteData routeData) => (string?) routeData.Values["page"];
    
    public static string? GetCurrentPage(this ViewContext view)
    {
        string? page = (string?) view.RouteData.Values["page"];
        return page;
    }
}