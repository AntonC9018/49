using System.ComponentModel.DataAnnotations;
using fourtynine.DataAccess;

namespace fourtynine.Postings;

#pragma warning disable 8618 // Disable nullability warnings

public sealed class PostingGetDto_General : IPostingIdentification
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ThumbnailUrl { get; set; }
    public DateTime DatePosted { get; set; }
 
    // TODO
    // public string Slug { get; set; }
}

public sealed class PostingDetailsDto
{
    public PricingPostingDetailsDto? Pricing { get; set; } 
    public VehiclePostingDetailsDto? Vehicle { get; set; } 
    public RealEstatePostingDetailsDto? RealEstate { get; set; } 
    public LocationPostingDetailsDto? Location { get; set; } 
}

public class PricingPostingDetailsDto : IPricingPostingDetails
{
    [Required] public BargainKinds BargainKinds { get; set; }
    public decimal? Price { get; set; }
    public decimal? PriceMax { get; set; }
}

public sealed class VehiclePostingDetailsDto : IVehiclePostingDetails
{
    [Required] public int Year { get; set; }
    [Required] public string Manufacturer { get; set; }
    [Required] public string Model { get; set; }
}

public sealed class RealEstatePostingDetailsDto : IRealEstatePostingDetails
{
    [Required] public RealEstateKind Kind { get; set; }
    [Required] public RealEstateSpacePurpose SpacePurpose { get; set; }
    [Required] public float Area { get; set; }
    [Required] public int Rooms { get; set; }
}

public sealed class LocationPostingDetailsDto : ILocationPostingDetails
{
    [Required] public string Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public sealed class PostingGetDto_Detailed : IPostingIdentification
{
    public PostingGetDto_General General { get; set; }
    public List<string> PictureUrls { get; set; }
    public PostingAuthorGetDto Author { get; set; }
    public PostingDetailsDto Details { get; set; }

    public long Id => General.Id;
    public string Title => General.Title;
}

public sealed class PostingAuthorGetDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public sealed class PostingCreateDto
{
    [Required] public string Title { get; set; }
    [Required] public string Description { get; set; }
    [Required] public string ThumbnailUrl { get; set; }
    [Required] public PostingDetailsDto Details { get; set; }
}

#pragma warning restore 8618 // Disable nullability warnings
