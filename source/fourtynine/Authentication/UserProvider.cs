using System.Security.Claims;
using fourtynine.DataAccess;
using Microsoft.EntityFrameworkCore;
using DbContext = fourtynine.DataAccess.DbContext;

namespace fourtynine.Authentication;

public class UserProviderService
{
    public DbContext DbContext { get; }
    public HttpContext HttpContext { get; }

    private bool _isUserCached;
    private ApplicationUser? _cachedUser; 

    public UserProviderService(DbContext dbContext, HttpContext httpContext)
    {
        DbContext = dbContext;
        HttpContext = httpContext;
    }
    
    public IQueryable<ApplicationUser> QueryUser()
    {
        return DbContext.Users.Where(x => x.Id == UserId);
    }

    public async ValueTask<ApplicationUser?> GetUser()
    {
        // We don't have to lock anything here since
        // this object is only used in scope of a single request.
        if (!_isUserCached)
        {
            _cachedUser = await QueryUser().FirstOrDefaultAsync();
            _isUserCached = true;
        }
        return _cachedUser;
    }

    public async ValueTask<ApplicationUser> GetAuthenticatedUser()
    {
        var user = await GetUser();
        if (user is null)
            throw new InvalidOperationException("User is not authenticated");
        return user;
    }
    
    public Guid UserId => HttpContext.User.GetId();
}

public static class UserProviderServiceExtensions
{
    public static Guid GetId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(userId);
    }
}