using System;

namespace fourtynine.DataAccess;

public interface IPostingIdentification
{
    long Id { get; }
    string Title { get; }
}

public enum PostingKind
{
    [TagForType(typeof(RealEstatePostingDetails))]
    RealEstate,

    [TagForType(typeof(VehiclePostingDetails))]
    Vehicle,
}

public sealed class TagForTypeAttribute : Attribute
{
    public TagForTypeAttribute(Type type)
    {
        Type = type;
    }

    public Type Type { get; set; }
}

public readonly record struct PriceRange(decimal Min, decimal Max);

public interface IPricingPostingDetails
{
    BargainKinds BargainKinds { get; set; }
    decimal? Price { get; set; }
    decimal? PriceMax { get; set; }
}

[Flags]
public enum BargainKinds
{
    Offer = 1 << 0,
    Request = 1 << 1,
    Permanent = 1 << 2,
    Temporary = 1 << 3,
    
    Sell = Offer | Permanent,
    Rent = Offer | Temporary,
    
    Buy = Request | Permanent,
    Lease = Request | Temporary,
    
    // In these cases the price is probably not going to be set.
    SaleOrRent = Sell | Rent,
    BuyOrLease = Buy | Lease,
    
    All = Offer | Request | Permanent | Temporary,
}

public static class PricingPostingDetailsExtensions
{
    public static bool IsPriceNegotiable(this IPricingPostingDetails i) => !i.Price.HasValue;

    public static PriceRange? GetPriceRange(this IPricingPostingDetails i)
    {
        if (i.Price is not null && i.PriceMax is not null)
            return new PriceRange(i.Price.Value, i.PriceMax.Value);
        return null;
    }

    public static void SetPriceRange(this IPricingPostingDetails i, PriceRange range)
    {
        i.Price = range.Min;
        i.PriceMax = range.Max;
    }
}

public interface IVehiclePostingDetails
{
    int Year { get; set; }
    string Manufacturer { get; set; }
    string Model { get; set; }
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

public interface IRealEstatePostingDetails
{
    RealEstateKind Kind { get; }
    RealEstateSpacePurpose SpacePurpose { get; }
    float Area { get; }
    int Rooms { get; }
}

public readonly record struct Coordinates(double Latitude, double Longitude); 


public interface ILocationPostingDetails
{
    public string Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public static class LocationPostingDetailsExtensions
{
    public static Coordinates? GetCoordinates(this ILocationPostingDetails i)
    {
        if (i.Latitude is not null && i.Longitude is not null)
            return new Coordinates(i.Latitude.Value, i.Longitude.Value);
        return null;
    }
    
    public static void SetCoordinates(this ILocationPostingDetails i, Coordinates? coordinates)
    {
        if (!coordinates.HasValue)
        {
            i.Latitude = null;
            i.Longitude = null;
        }
        else
        {
            i.Latitude = coordinates.Value.Latitude;
            i.Longitude = coordinates.Value.Longitude;
        }
    }
}
