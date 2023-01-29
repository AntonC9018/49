#if !USE_YARP
using AspNetCore.Proxy;
#endif
using AspNet.Security.OAuth.GitHub;
using AutoMapper;
using FluentValidation;
using fourtynine;
using fourtynine.Authentication;
using fourtynine.DataAccess;
using fourtynine.Development;
using fourtynine.Navbar;
using fourtynine.Postings;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Net.Http.Headers;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using DbContext = fourtynine.DataAccess.DbContext;
// ReSharper disable VariableHidesOuterVariable

var builder = WebApplication.CreateBuilder(args);
bool isDevelopment = builder.Environment.IsDevelopment();

// Add services to the container.
var mvcBuilder = builder.Services.AddControllers(options =>
{
}).AddJsonOptions(options =>
{
    // Keep the source casing.
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.DictionaryKeyPolicy = null;
    options.JsonSerializerOptions.WriteIndented = isDevelopment;
});

{
    var modelBuilder = new ODataConventionModelBuilder();
    modelBuilder.EntitySet<PostingGetDto_Detailed>("Postings");
    modelBuilder.EntitySet<PostingAuthorGetDto>("Authors");
    
    // I wonder why this can't be deduced from the EF Core config?
    // I should just need to say that these types are DTOs of the EF core entity types.
    modelBuilder.EntityType<PostingGetDto_Detailed>()
        .HasKey(a => a.Id);

    modelBuilder.EntityType<PostingGetDto_Detailed>()
        .HasOptional(a => a.Author);
    
    modelBuilder.EntityType<PostingAuthorGetDto>()
        .HasKey(a => a.Id);

    modelBuilder.ComplexType<PostingGetDto_General>();
    
    mvcBuilder.AddOData(options =>
    {
        options.EnableQueryFeatures();
        options.AddRouteComponents("odata", modelBuilder.GetEdmModel());
    });
}

// Swagger OData integration
// https://shawn-shi.medium.com/clean-architecture-rest-api-with-odata-and-swagger-ui-406f7df896c
mvcBuilder.AddMvcOptions(options =>
{
    var odataMediaType = new MediaTypeHeaderValue("application/prs.odatatestxx-odata");
    
    foreach (var outputFormatter in options.OutputFormatters
        .OfType<OutputFormatter>()
        .Where(x => x.SupportedMediaTypes.Count == 0))
    {
        outputFormatter.SupportedMediaTypes.Add(odataMediaType);
    }

    foreach (var inputFormatter in options.InputFormatters
        .OfType<InputFormatter>()
        .Where(x => x.SupportedMediaTypes.Count == 0))
    {
        inputFormatter.SupportedMediaTypes.Add(odataMediaType);
    }
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

builder.Services.AddDbContext<DbContext>(options =>
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
    options.AddProfile(PostingMapperProfile.Instance);
});

// I'm registering a separate mapper that's meant to be used for OData.
// Might want to write some more code for better configuration
// (basically copy-pasting some things from the automapper DI impl).
builder.Services.AddSingleton<IODataMapperProvider>(sp =>
{
    var config = new MapperConfiguration(options =>
    {
        options.AddProfile(ODataPostingMapperProfile.Instance);
    });
    var mapper = config.CreateMapper(sp.GetService);
    return new ODataMapperProvider(mapper);
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
   
    // Doesn't actually seem to have an effect, might be a bug.
    // The generated client still accepts nulls even if the properties aren't nullable.
    // The MVC validation seems to rely on the nullability instead (since it has the
    // SuppressImplicitRequiredAttributeForNonNullableReferenceTypes configuration property),
    // hence by default the swagger schema doesn't reflect the actual API fully.
    // options.SupportNonNullableReferenceTypes();
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

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password = null;
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<DbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserProviderService>();
{
    var auth = builder.ConfigureAuthenticationSources();
    auth.AddGithubAuthentication(builder.Configuration);
    auth.AddGoogleAuthentication(builder.Configuration);

    builder.Services.AddKeyed<IUserEmailConfirmationProvider>(options =>
    {
        options.Add<GithubEmailConfirmationProvider>(
            GitHubAuthenticationDefaults.AuthenticationScheme, ServiceLifetime.Singleton);
        options.Add<GoogleEmailConfirmationProvider>(
            GoogleDefaults.AuthenticationScheme, ServiceLifetime.Singleton);
    });
}

builder.Services.AddAuthorization(options =>
{
});

builder.Services.AddValidatorsFromAssembly(
    typeof(PostingCreateDtoValidator).Assembly,
    lifetime: ServiceLifetime.Singleton);
builder.Services.AddFluentValidationRulesToSwagger();

// We cannot use client side validation for postings, because we have some nested
// objects that are only active according to some other flags.
// Since the client side validation cannot handle checks, like When clauses in fluent validation,
// there's no way to apply some rules only conditionally.
// This means client side validation will either have to be done manually, or we have to settle for a
// library to do said validation on the server.
// https://github.com/sinanbozkus/FormHelper
// builder.Services.AddFluentValidationClientsideAdapters();

// register send grid email service
builder.Services.AddSingleton<SendGridEmailSender>();
builder.Services.AddSingleton<IEmailSender>(sp => sp.GetRequiredService<SendGridEmailSender>());
builder.Services.AddSingleton<ISendGridEmailSender>(sp => sp.GetRequiredService<SendGridEmailSender>());

builder.Services.Configure<SendGridEmailSenderOptions>(o =>
{
    o.Key = builder.Configuration["SendGridKey"];
    o.SenderEmail = builder.Configuration["SendGridSenderEmail"];
    o.SenderName = builder.Configuration["SendGridSenderName"];
});

builder.Services.AddHttpContextAccessor();


var app = builder.Build();

if (isDevelopment)
    app.EnsureDatabaseCreated<DbContext>();

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

if (isDevelopment)
    app.UseODataRouteDebug();

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
    
    endpoints.MapGet("/Account/Logout", async context =>
    {
        var authSchemeUsed = AuthTokenSources.AuthCookieName;
        await context.SignOutAsync(
            authSchemeUsed,
            new AuthenticationProperties
            {
                RedirectUri = "/"
            });
    });
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
