using System.ComponentModel.DataAnnotations;
using fourtynine.DataAccess;

namespace fourtynine.Postings;

#pragma warning disable 8618 // Disable nullability warnings

public sealed class PostingGetDto_General : IPostingIdentification
{
    [Required] public long Id { get; set; }
    [Required] public string Title { get; set; }
    [Required] public string Description { get; set; }
    [Required] public string ThumbnailUrl { get; set; }
    [Required] public DateTime DatePosted { get; set; }
    
    // NOTE: has to be initialized manually if it's needed.
    // TODO: Maybe make the computed properties lazy?
    [Required] public string? Slug { get; set; }
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
    [Required] public PostingGetDto_General General { get; set; }
    [Required] public List<string> PictureUrls { get; set; }
    [Required] public PostingAuthorGetDto Author { get; set; }
    [Required] public PostingDetailsDto Details { get; set; }

    long IPostingIdentification.Id => General.Id;
    string IPostingIdentification.Title => General.Title;
}

public sealed class PostingAuthorGetDto
{
    [Required] public int Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] public string Email { get; set; }
}

public sealed class PostingCreateDto
{
    [Required] public string Title { get; set; }
    [Required] public string Description { get; set; }
    [Required] public string ThumbnailUrl { get; set; }
    [Required] public PostingDetailsDto Details { get; set; }
}

#pragma warning restore 8618 // Disable nullability warnings
