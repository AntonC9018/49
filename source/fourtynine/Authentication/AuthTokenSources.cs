using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace fourtynine.Authentication;

public static class AuthTokenSources
{
    public const string AnySchemeName = "PolicyScheme";
    public const string AuthCookieName = "CookieAuth";
    public const string AuthJwtName = "JwtAuth";

    public static AuthenticationBuilder ConfigureAuthenticationSources(this WebApplicationBuilder builder)
    {
        return builder.Services.AddAuthentication(defaultScheme: AnySchemeName)
            .AddCookie(AuthCookieName, ConfigureCookie)
            // Will do this one later.
            // .AddJwtBearer(AuthJwtName, o => ConfigureJwt(o, builder.Configuration))
            .AddPolicyScheme(AnySchemeName, displayName: null, options =>
            {
                options.ForwardDefaultSelector = context => SelectAuthScheme(context.Request);
            });
    }
    
    private static string SelectAuthScheme(HttpRequest request)
    {
        var cookie = request.Cookies[AuthCookieName];
        if (cookie != null)
            return AuthCookieName;
        
        var hasBearer = request.Headers.Authorization
            .FirstOrDefault()?.StartsWith("Bearer ") ?? false;
        if (hasBearer)
            return AuthJwtName;
        
        return request.Path.IsApi() ? AuthJwtName : AuthCookieName;
    }
    
    private static void ConfigureCookie(CookieAuthenticationOptions options)
    {
        options.Cookie.Name = AuthCookieName;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
        var expirationTime = TimeSpan.FromDays(200);
        options.ExpireTimeSpan = expirationTime;
        // options.Cookie.Expiration = expirationTime; 
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";

        // Only redirect non-api requests.
        // The API requests made in the browser use the cookie rather than json bearer.
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.IsApi())
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            else
                context.Response.Redirect(context.RedirectUri);

            return Task.CompletedTask;
        };
    }

    private static void ConfigureJwt(JwtBearerOptions options, IConfiguration configuration)
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // TODO: configure this.
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
            
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };
    }
}