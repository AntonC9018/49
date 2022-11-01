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