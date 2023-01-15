using System.Security.Claims;
using fourtynine.DataAccess;

namespace fourtynine.Authentication;

public interface IApplicationUserStore
{
    /// <summary>
    /// This represents a user retrieved from the database.
    /// Stored here so that they don't have to be queried again,
    /// since the query logic depends on the authentication provider used and is quite complex.
    /// This not being set means the user is not logged in and should be enforced
    /// by the code that logs them in.
    /// </summary>
    ApplicationUser? User { get; set; }
}

public class ApplicationUserStore : IApplicationUserStore
{
    public ApplicationUser? User { get; set; }
}