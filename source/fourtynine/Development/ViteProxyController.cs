// This is the other implementation, which doesn't quite work
// If I use this implementation, the other api controllers never get hit
// for some reason. I have no idea why honestly: the order of this proxy route
// is larger than the order of the other routes, so it should be hit last.
#if DEVELOPMENT && !USE_YARP && false

using AspNetCore.Proxy;
using Microsoft.AspNetCore.Mvc;

namespace fourtynine.Development;

[ApiExplorerSettings(IgnoreApi = true)]
public class ViteProxyController : Controller
{
    const string _VitePort = "5173";

    [HttpGet("/{**catchAll}", Order = 99999999)]
    public Task GetAssets([FromRoute] string catchAll)
    {
        var url = $"https://localhost:{_VitePort}/{catchAll}";
        return HttpContext.HttpProxyAsync(url);
    }

    [HttpGet("/vite-ws")]
    public Task ViteWebsockets()
    {
        const string endpoint = $"wss://localhost:{_VitePort}/vite-ws";
        return HttpContext.WsProxyAsync(endpoint);
    }
}

#endif