using Microsoft.EntityFrameworkCore;

namespace fourtynine;

public static partial class DatabaseHelper
{
    public static void EnsureDatabaseCreated<TDbContext>(this WebApplication app)
        where TDbContext : DbContext
    {
        using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<TDbContext>();
        context.Database.EnsureCreated();
    }
}