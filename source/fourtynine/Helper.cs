using System;
using System.Data.Common;
using System.Diagnostics;
using System.Net.Mime;
using fourtynine.Navbar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
}

public sealed class ApiRoute : RouteAttribute
{
    public ApiRoute(string template = "api/[controller]") : base(template)
    {
    }
}

// Enables API conventions, like [ApiController],
// while also applying the default content types.
public sealed class ApiControllerConventionAttribute : Attribute,
    IControllerModelConvention,
    IApiBehaviorMetadata
{
    public void Apply(ControllerModel controller)
    {
        if (controller.Filters.All(f => f is not IResourceFilter))
            controller.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
        
        if (controller.Filters.All(f => f is not IResultFilter))
            controller.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
    }
}


// Not gonna work, because there is stuff in /public which doesn't end up in the manifest.
// public class ViteManifestReverseProxyFilter : IProxyConfigFilter
// {
//     private HashSet<string> ManifestKeys;
//     
//     public ValueTask<ClusterConfig> ConfigureClusterAsync(ClusterConfig cluster, CancellationToken cancel)
//     {
//         return new(cluster);
//     }
//
//     public ValueTask<RouteConfig> ConfigureRouteAsync(RouteConfig route, ClusterConfig? cluster, CancellationToken cancel)
//     {
//     }
// } 