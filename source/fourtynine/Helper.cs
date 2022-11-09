using System;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.AspNetCore.SpaServices.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace fourtynine;

public static class RoutingHelper
{
    public static string ControllerName(string nameMaybeWithControllerPart)
    {
        string result = nameMaybeWithControllerPart;
        if (result.EndsWith("Controller"))
            result = result[.. ^"Controller".Length];
        return result;
    }
    public static string ControllerName(System.Type controllerType)
    {
        return ControllerName(controllerType.Name);
    }
}

public class ApiRoute : RouteAttribute
{
    public ApiRoute(string template = "api/[controller]") : base(template)
    {
    }
}

// Enables API conventions, like [ApiController],
// but also applies the default content types.
public class ApiControllerConventionAttribute : Attribute,
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

public static class ViteHelper
{
    // The server has to be run manually in the right folder.
    // https://github.com/MakotoAtsu/AspNetCore_Vite_Template/blob/master/AspNetCore_Vite_Starter/Net6_MinimalAPI_And_Vite/ViteHelper.cs
    public static void UseViteDevelopmentServer(this ISpaBuilder spa, int? port = null)
    {
        int port_ = port.HasValue ? port.Value : 5173;
        spa.Options.DevServerPort = port_;
        
        var devServerEndpoint = new Uri($"https://localhost:{port_}");
        spa.UseProxyToSpaDevelopmentServer(devServerEndpoint);
    }
}