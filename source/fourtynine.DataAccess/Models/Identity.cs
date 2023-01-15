using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace fourtynine.DataAccess;

#pragma warning disable 8618 // Disable nullability warnings for EF Core

// Future-proof these things by making a subclass of everything.
// Also them being sealed could add a bit of performance.

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public ICollection<Posting> Postings { get; set; }
}

public sealed class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }
}

public sealed class ApplicationUserClaim : IdentityUserClaim<Guid>
{
}

public sealed class ApplicationUserLogin : IdentityUserLogin<Guid>
{
}

public sealed class ApplicationUserRole : IdentityUserRole<Guid>
{
}

public sealed class ApplicationUserToken : IdentityUserToken<Guid>
{
}

public sealed class ApplicationRoleClaim : IdentityRoleClaim<Guid>
{
} 

public sealed class ApplicationIdentityRole : IdentityRole<Guid>
{
    public ApplicationIdentityRole()
    {
    }

    public ApplicationIdentityRole(string roleName) : base(roleName)
    {
    }
}

#pragma warning restore 8618 // Disable nullability warnings for EF Core
