using System.Net.Mime;
using fourtynine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.SpaServices.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
});

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddDirectoryBrowser();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
    // In development static files are served by vite.
    var staticFilesPath = "StaticFiles";
    var requestPath = "/" + staticFilesPath;
    var staticFilesProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, staticFilesPath));
    
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = staticFilesProvider,
        RequestPath = requestPath,
    });

    app.UseDirectoryBrowser(new DirectoryBrowserOptions
    {
        FileProvider = staticFilesProvider,
        RequestPath = requestPath,
    });
}

if (app.Environment.IsDevelopment())
{
    IFileInfo j;
    app.UseSpaStaticFiles();
    
    app.UseSpa(spa =>
    {
        spa.UseViteDevelopmentServer();
    });
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
