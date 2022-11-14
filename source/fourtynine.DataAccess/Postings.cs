using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace fourtynine.DataAccess;

#pragma warning disable 8618 // Disable nullability warnings for EF Core

public sealed class Posting
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

    public ICollection<string> PictureUrls { get; set; }
    
    [Required]
    public int AuthorId { get; set; }
    public Author Author { get; set; }
    
    // It's more memory efficient and faster to query if all of the categories
    // data is stored in a single entity, rather than split across multiple tables.
    // TBH vs TBT, TBH basically always wins.
    // https://www.youtube.com/watch?v=HaL6DKW1mrg
    // Now, the link does apply to inheritance, but it's the same idea here.
    // We could also store a tag to be able to find out the category easier, and be able to
    // make indices easier, but for now it's fine to just check for nulls.
    // These types being owned makes the storage TBH essentially, all denormalized, with nulls.
    public PricingPostingDetails? Pricing { get; set; }
    public LocationPostingDetails? Location { get; set; }
    public RealEstatePostingDetails? RealEstate { get; set; }
    public VehiclePostingDetails? Vehicle { get; set; }
    
    // Examples:
    //
    // Monetary + Location + RealEstate = 
    // Selling/Buying/Etc. some real estate at a given location, Vehicle must be null.
    //
    // Monetary + Vehicle =
    // Selling/Buying/Etc. some vehicle, without an explicit address.
    // 
}

[Flags]
public enum MonetaryStatus
{
    Offer = 1 << 0,
    Request = 1 << 1,
    Permanent = 1 << 2,
    Temporary = 1 << 3,
    
    Sale = Offer | Permanent,
    Rent = Offer | Temporary,
    
    Buy = Request | Permanent,
    Lease = Request | Temporary,
    
    // In these cases the price is probably not going to be set.
    SaleOrRent = Sale | Rent,
    BuyOrLease = Buy | Lease,
}

public readonly record struct PriceRange(decimal Min, decimal Max);

[Owned]
public sealed class PricingPostingDetails
{
    [Column(TypeName = "byte")]
    public MonetaryStatus Status { get; set; }
    
    [Column(TypeName = "money")]
    public decimal? Price { get; set; }
    
    [Column(TypeName = "money")]
    [NotNullIfNotNull(nameof(Price))]
    public decimal? PriceMax { get; set; }
    
    [MemberNotNullWhen(returnValue: false, nameof(Price))]
    public bool IsPriceNegotiable => !Price.HasValue;
    
    [MemberNotNullWhen(returnValue: true, nameof(Price))]
    public bool HasPrice => Price.HasValue;
    
    [MemberNotNullWhen(returnValue: true, nameof(Price), nameof(PriceMax))]
    public bool HasPriceRange => Price is not null && PriceMax is not null;
    
    // We have to do bullshit like this, because mapping a value type to multiple columns
    // is not supported in either EF Core 6 or 7, it's been postponed until version 8.
    public PriceRange PriceRange
    {
        get
        {
            Debug.Assert(HasPriceRange,
            "Must check IsPriceNegotiable prior to taking the price minimum.");
            return new PriceRange(Price!.Value, PriceMax.Value);
        }
        set
        {
            Price = value.Min;
            PriceMax = value.Max;
        }
    }
}

[Owned]
public sealed class VehiclePostingDetails
{
    public int Year { get; set; }
    
    [MaxLength(100)]
    public string Manufacturer { get; set; }
    
    [MaxLength(100)]
    public string Model { get; set; }
}

public enum RealEstateKind
{
    House,
    Apartment,
    Condo,
    Townhouse,
    Land,
    Other,
}

public enum RealEstateSpacePurpose
{
    Residential,
    Commercial,
    Industrial,
    Any,
    Other,
}

[Owned]
public sealed class RealEstatePostingDetails
{
    [Column(TypeName = "varchar(16)")]
    public RealEstateKind Kind { get; set; }

    [Column(TypeName = "varchar(16)")]
    public RealEstateSpacePurpose SpacePurpose { get; set; }
    
    [Precision(8, 1)]
    public float Area { get; set; }

    public int Rooms { get; set; }
}

public readonly record struct Coordinates(float Latitude, float Longitude); 

[Owned]
public sealed class LocationPostingDetails
{
    [MaxLength(100)]
    public string Country { get; set; }
    
    [MaxLength(100)]
    [NotNullIfNotNull(nameof(Address))]
    public string? City { get; set; }
    
    [MaxLength(100)]
    public string? Address { get; set; }

    [NotNullIfNotNull(nameof(Longitude))]
    public float? Latitude { get; set; }

    [NotNullIfNotNull(nameof(Latitude))]
    public float? Longitude { get; set; }

    [MemberNotNullWhen(returnValue: true, nameof(Latitude))]
    public bool HasCoordinates => Latitude.HasValue;

    public Coordinates Coordinates
    {
        get
        {
            Debug.Assert(HasCoordinates, 
                "Check if it has coordinates prior to getting them.");
            return new Coordinates(Latitude!.Value, Longitude!.Value);
        }
        set
        {
            Latitude = value.Latitude;
            Longitude = value.Longitude;
        }
    }
}

// [Owned]
// public sealed class ShippingPostingDetails
// {
// }


#pragma warning enable 8618 // Disable nullability warnings for EF Core

public sealed class PostingsDbContext : DbContext
{
    public DbSet<Posting> Postings { get; set; }
    public DbSet<Author> Authors { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Posting>()
            .OwnsOne(p => p.Pricing);
    }
}

