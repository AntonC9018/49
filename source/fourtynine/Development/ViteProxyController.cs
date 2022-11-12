#if DEVELOPMENT
using System.Diagnostics;
using AspNetCore.Proxy;
using Microsoft.AspNetCore.Mvc;

namespace fourtynine;

public class ViteProxyController : Controller
{
    const string _VitePort = "5173";

    [Route("/{**catchAll}", Order = 99999999)]
    [HttpGet]
    public Task GetAssets([FromRoute] string catchAll)
    {
        var url = $"https://localhost:{_VitePort}/{catchAll}";
        return HttpContext.HttpProxyAsync(url);
    }

    [Route("/vite-ws")]
    public Task ViteWebsockets()
    {
        const string endpoint = $"wss://localhost:{_VitePort}/vite-ws";
        return HttpContext.WsProxyAsync(endpoint);
    }
}

#endif