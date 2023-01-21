using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace fourtynine;


public static class ApiRouteHelper
{
    public static bool IsApi(this PathString path)
    {
        return path.StartsWithSegments("/api");
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