﻿using System;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

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

public class ApiControllerConventionAttribute : Attribute,
    IControllerModelConvention,
    IApiBehaviorMetadata,
    IRouteTemplateProvider
{
    public void Apply(ControllerModel controller)
    {
        if (controller.Filters.All(f => f is not IResourceFilter))
            controller.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
        
        if (controller.Filters.All(f => f is not IResultFilter))
            controller.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));

        Name ??= controller.ControllerName;
    }

    public string? Template { get; set; } = "api/[controller]";
    public int? Order { get; set; }
    public string? Name { get; set; }
}