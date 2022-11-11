using AspNetCore.Proxy;
using fourtynine.Navbar;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
bool isDevelopment = builder.Environment.IsDevelopment();

// Add services to the container.
builder.Services.AddControllersWithViews();

if (!isDevelopment)
    builder.Services.AddDirectoryBrowser();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "49 API",
        Description = "An e-commerce application",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
});

if (isDevelopment)
{
    builder.Services.AddProxies();
}

builder.Services.AddSingleton<INavbarActionsService, NavbarActionsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!isDevelopment)
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (!isDevelopment)
{
    // In development static files are served by vite.
    var staticFilesPath = "StaticFiles";
    var staticFilesProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, staticFilesPath));
    
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = staticFilesProvider,
        RequestPath = "/StaticFiles",
    });

    app.UseDirectoryBrowser(new DirectoryBrowserOptions
    {
        FileProvider = staticFilesProvider,
        RequestPath = "/FileBrowser",
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();

if (isDevelopment)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllers();
});

if (isDevelopment)
{
    const string vitePort = "5173";

    app.UseProxies(proxies =>
    {
        proxies.Map("/vite-ws", proxy => proxy
            .UseWs($"wss://localhost:{vitePort}/vite-ws"));
        proxies.Map("/{**all}", proxy => proxy
            .UseHttp((_, args) => $"https://localhost:{vitePort}/{args["all"]}"));
    });
}

app.Run();
