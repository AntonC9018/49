using System.Net.Mime;
using fourtynine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
bool isDevelopment = builder.Environment.IsDevelopment();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
});

if (!isDevelopment)
    builder.Services.AddDirectoryBrowser();

if (isDevelopment)
{
    var proxy = builder.Services.AddReverseProxy();
    proxy.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (isDevelopment)
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
        RequestPath = "/",
    });

    app.UseDirectoryBrowser(new DirectoryBrowserOptions
    {
        FileProvider = staticFilesProvider,
        RequestPath = "/filebrowser",
    });
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

if (isDevelopment)
    app.MapReverseProxy();

app.Run();
