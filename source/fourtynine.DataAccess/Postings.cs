using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace fourtynine.DataAccess;

#pragma warning disable 8618 // Disable nullability warnings for EF Core

public sealed class Posting : IPostingIdentification
{
    [Required]
    public long Id { get; set; }
    
    [MaxLength(100)]
    public string Title { get; set; }
    
    [MaxLength(1000)]
    public string Description { get; set; }

    [MaxLength(100)]
    public string ThumbnailUrl { get; set; }
    
    public DateTime DatePosted { get; set; }

    public ICollection<Picture> Pictures { get; set; }
    
    [Required]
    public int AuthorId { get; set; }
    public Author Author { get; set; }
    
    // It's more memory efficient and faster to query if all of the categories
    // data is stored in a single entity, rather than split across multiple tables.
    // TBH vs TBT, TBH basically always wins.
    // https://www.youtube.com/watch?v=HaL6DKW1mrg
    // Now, even though the link applies to inheritance, it's the same idea here.
    // We could also store a tag to be able to find out the category easier, and be able to
    // make indices easier, but for now it's fine to just check for nulls.
    // These types being owned makes the storage TBH essentially, all denormalized, with nulls.
    public PricingPostingDetails? Pricing { get; set; }
    public LocationPostingDetails? Location { get; set; }
    
    
    // The things below always act like a discriminated union,
    // only one of the can be not null.
    // The correctness of Kind is enforced from the outside.
    public PostingKind Kind { get; set; }
    public RealEstatePostingDetails? RealEstate { get; set; }
    public VehiclePostingDetails? Vehicle { get; set; }
    
    // Examples:
    //
    // Pricing + Location + RealEstate = 
    // Selling/Buying/Etc. some real estate at a given location, Vehicle must be null.
    //
    // Pricing + Vehicle =
    // Selling/Buying/Etc. some vehicle, without an explicit address.
    // 
}

public sealed class Picture
{
    public long Id { get; set; }
    public string Url { get; set; }
    public long PostingId { get; set; }
    public Posting Posting { get; set; }
}

[Owned]
public sealed class PricingPostingDetails : IPricingPostingDetails
{
    [Column(TypeName = "tinyint")]
    public BargainKinds BargainKinds { get; set; }
    
    [Column(TypeName = "money")]
    public decimal? Price { get; set; }
    
    [Column(TypeName = "money")]
    public decimal? PriceMax { get; set; }
}

[Owned]
public sealed class VehiclePostingDetails : IVehiclePostingDetails
{
    public int Year { get; set; }
    
    [MaxLength(100)]
    public string Manufacturer { get; set; }
    
    [MaxLength(100)]
    public string Model { get; set; }
}

[Owned]
public sealed class RealEstatePostingDetails : IRealEstatePostingDetails
{
    [Column(TypeName = "varchar(16)")]
    public RealEstateKind Kind { get; set; }

    [Column(TypeName = "varchar(16)")]
    public RealEstateSpacePurpose SpacePurpose { get; set; }
    
    // [Precision(8, 1)]
    [Column(TypeName = "decimal(8, 1)")]
    public float Area { get; set; }

    public int Rooms { get; set; }
}

[Owned]
public sealed class LocationPostingDetails : ILocationPostingDetails
{
    [MaxLength(100)]
    public string Country { get; set; }
    
    [MaxLength(100)]
    [NotNullIfNotNull(nameof(Address))]
    public string? City { get; set; }
    
    [MaxLength(100)]
    public string? Address { get; set; }

    [NotNullIfNotNull(nameof(Longitude))]
    public double? Latitude { get; set; }

    [NotNullIfNotNull(nameof(Latitude))]
    public double? Longitude { get; set; }
}

// [Owned]
// public sealed class ShippingPostingDetails
// {
// }

public sealed class PostingsDbContext : DbContext
{
    public DbSet<Posting> Postings { get; set; }
    public DbSet<Author> Authors { get; set; }
    
    public PostingsDbContext(DbContextOptions<PostingsDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Posting>()
            .OwnsOne(p => p.Pricing);

        modelBuilder.Entity<Posting>()
            .HasMany(p => p.Pictures)
            .WithOne(p => p.Posting);
    }
}

#pragma warning restore 8618 // Disable nullability warnings for EF Core
