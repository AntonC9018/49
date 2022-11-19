using System.Diagnostics;
using System.Text.Json;
using AspNetCore.Proxy;
using fourtynine;
using fourtynine.DataAccess;
using fourtynine.Development;
using fourtynine.Navbar;
using fourtynine.Postings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

var builder = WebApplication.CreateBuilder(args);
bool isDevelopment = builder.Environment.IsDevelopment();

// Add services to the container.
builder.Services.AddControllers(options =>
{
}).AddJsonOptions(options =>
{
    // Keep the source casing.
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.DictionaryKeyPolicy = null;
});

builder.Services.AddRazorPages()
    .WithRazorPagesRoot("/Pages")
    // Razor pages don't recompile = css in the html doesn't update.
    .AddRazorRuntimeCompilation();

builder.Services.AddDbContext<PostingsDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Postings");
    
    // Could use the secrets manager to configure passwords and stuff.
    // I guess, the user name would be configured in here too.
    // https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows#string-replacement-with-secrets
    // var b = new SqlConnectionStringBuilder(connectionString);
    // connectionString = b.ConnectionString;
    
    
    options.UseSqlServer(connectionString);
});

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
        },
    });
    options.AddEnumsWithValuesFixFilters();
});

if (isDevelopment)
{
    builder.Services.AddProxies();
}

builder.Services.AddSingleton<INavbarActionsService, NavbarActionsService>();

if (isDevelopment)
{
    builder.Services.AddSingleton<IViteManifestService, ViteManifestIdentityMappingService>();
}
else
{
    // Could also instantiate this dynamically based on the environment.
    var manifestPath = Path.Join(ProjectConfiguration.StaticFilesFolderRelativePath, "manifest.json");
    var manifestService = new ViteManifestService(manifestPath);
    builder.Services.AddSingleton<IViteManifestService>(manifestService);
}

builder.Services.AddScoped<PostingApiService>();

var app = builder.Build();

if (isDevelopment)
    app.EnsureDatabaseCreated<PostingsDbContext>();

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
    app.UseStaticFiles();

    app.UseDirectoryBrowser(new DirectoryBrowserOptions
    {
        FileProvider = builder.Environment.WebRootFileProvider,
        RequestPath = "/FileBrowser",
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseWebSockets();

if (isDevelopment)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    
    // endpoints.MapControllerRoute(
    //     name: "default",
    //     pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllers();
});

app.Run();
