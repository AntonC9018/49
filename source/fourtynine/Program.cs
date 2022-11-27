#if !USE_YARP
using AspNetCore.Proxy;
#endif
using System.Net;
using FluentValidation;
using fourtynine;
using fourtynine.DataAccess;
using fourtynine.Development;
using fourtynine.Navbar;
using fourtynine.Postings;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
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

#if USE_YARP
if (isDevelopment)
{
    var proxy = builder.Services.AddReverseProxy();
    proxy.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
}
#endif

{
    var razor = builder.Services.AddRazorPages()
        .WithRazorPagesRoot("/Pages");
    
    if (isDevelopment)
    {
        // Razor pages don't recompile = css in the html doesn't update.
        razor.AddRazorRuntimeCompilation();
    }
}

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

builder.Services.AddAutoMapper(options =>
{
    options.AddProfile<PostingMapperProfile>();
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
    options.CustomSchemaIds(t => t.Name
        .Replace("Dto_", "")
        .Replace("Dto", ""));
    options.AddEnumsWithValuesFixFilters();
});

#if !USE_YARP
if (isDevelopment)
    builder.Services.AddProxies();
#endif

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

builder.Services.AddAuthentication(defaultScheme: ProjectConfiguration.AuthCookieName)
    .AddCookie(ProjectConfiguration.AuthCookieName, options =>
    {
        options.Cookie.Name = ProjectConfiguration.AuthCookieName;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
        
        // Only redirect non-api requests.
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            else
                context.Response.Redirect(context.RedirectUri);

            return Task.CompletedTask;
        };
    });

builder.Services.AddValidatorsFromAssembly(
    typeof(PostingCreateDtoValidator).Assembly,
    lifetime: ServiceLifetime.Singleton);
builder.Services.AddFluentValidationRulesToSwagger();

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
app.UseWebSockets();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

if (isDevelopment)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllers();
    
#if USE_YARP
    if (isDevelopment)
        endpoints.MapReverseProxy();
#endif
});

// The minimal API implementation seems to be the only thing that works correctly.
// It allows the api controllers to get hit, it allows swagger to load, it
// lets the razor pages work, and it proxies the other requests correctly too, including
// the websocket requests.
// See the controller implementation in `Development/ViteProxyController.cs`
#if !USE_YARP
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
#endif

app.Run();
