using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace fourtynine.DataAccess;

#pragma warning disable 8618 // Disable nullability warnings for EF Core

public sealed class DbContext : IdentityDbContext<
    ApplicationUser, ApplicationRole, Guid,
    ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
    ApplicationRoleClaim, ApplicationUserToken>
{
    public DbSet<Posting> Postings { get; set; }
    public DbSet<ApplicationUser> Authors => Users;
    
    public DbContext(DbContextOptions<DbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Posting>()
            .OwnsOne(p => p.Pricing);

        modelBuilder.Entity<Posting>()
            .HasMany(p => p.Pictures)
            .WithOne(p => p.Posting);
        
        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Postings)
            .WithOne(p => p.Author)
            .HasForeignKey(p => p.AuthorId);
        
        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.AllowedAuthenticationSchemes)
            .WithOne()
            .HasForeignKey(s => s.UserId);
        
        modelBuilder.Entity<AllowedAuthenticationScheme>()
            .HasKey(s => new { s.UserId, s.SchemeName });
    }
}

#pragma warning restore 8618 // Disable nullability warnings for EF Core
