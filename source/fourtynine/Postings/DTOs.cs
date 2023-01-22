using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using FluentValidation;
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
    public LocationPostingDetailsDto? Location { get; set; }
    
    [Required] public PostingKind Kind { get; set; }
    public RealEstatePostingDetailsDto? RealEstate { get; set; } 
    public VehiclePostingDetailsDto? Vehicle { get; set; } 
}

public sealed class PricingPostingDetailsDto : IPricingPostingDetails
{
    [Required] public BargainKinds BargainKinds { get; set; }
    public decimal? Price { get; set; }
    public decimal? PriceMax { get; set; }
}

public sealed class PricingPostingDetailsDtoValidator : AbstractValidator<PricingPostingDetailsDto>
{
    public PricingPostingDetailsDtoValidator()
    {
        RuleFor(x => x.BargainKinds)
            .Must(x => ((x & BargainKinds.Offer) != 0) != ((x & BargainKinds.Request) != 0))
            .WithMessage("No buying and selling at the same time.");
        
        RuleFor(x => x.BargainKinds)
            .Must(x => (x & (BargainKinds.Offer | BargainKinds.Request)) != 0)
            .WithMessage("Either offer or request is required.");
        
        RuleFor(x => x.BargainKinds)
            .Must(x => x < BargainKinds.All);
        
        RuleFor(x => x.Price)
            .NotNull().When(x => x.PriceMax is not null);
    }
}


public sealed class VehiclePostingDetailsDto : IVehiclePostingDetails
{
    [Required] public int Year { get; set; }
    [Required] public string Manufacturer { get; set; }
    [Required] public string Model { get; set; }
}

public sealed class VehiclePostingDetailsDtoValidator : AbstractValidator<VehiclePostingDetailsDto>
{
    public VehiclePostingDetailsDtoValidator()
    {
        RuleFor(x => x.Year)
            .Must(x => x >= 1900 && x <= DateTime.Now.Year);
        RuleFor(x => x.Manufacturer)
            .NotEmpty();
        RuleFor(x => x.Model)
            .NotEmpty();
    }
}

public sealed class RealEstatePostingDetailsDto : IRealEstatePostingDetails
{
    [Required] public RealEstateKind Kind { get; set; }
    [Required] public RealEstateSpacePurpose SpacePurpose { get; set; }
    [Required] public float Area { get; set; }
    [Required] public int Rooms { get; set; }
}

public sealed class RealEstatePostingDetailsDtoValidator : AbstractValidator<RealEstatePostingDetailsDto>
{
    public RealEstatePostingDetailsDtoValidator()
    {
        RuleFor(x => x.Kind)
            .IsInEnum();
        RuleFor(x => x.SpacePurpose)
            .IsInEnum();
        RuleFor(x => x.Area)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.Rooms)
            .GreaterThan(0);
    }
}

public sealed class LocationPostingDetailsDto : ILocationPostingDetails
{
    [Required] public string Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public sealed class LocationPostingDetailsDtoValidator : AbstractValidator<LocationPostingDetailsDto>
{
    public LocationPostingDetailsDtoValidator()
    {
        RuleFor(x => x.City)
            .NotNull().When(x => x.Address is not null);
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).When(x => x.Latitude is not null);
        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).When(x => x.Longitude is not null);
    }
}


public sealed class PostingDetailsDtoValidator : AbstractValidator<PostingDetailsDto>
{
    public PostingDetailsDtoValidator(
        IValidator<RealEstatePostingDetailsDto> realEstatePostingDetailsDtoValidator,
        IValidator<VehiclePostingDetailsDto> vehiclePostingDetailsDtoValidator,
        IValidator<PricingPostingDetailsDto> pricingPostingDetailsDtoValidator,
        IValidator<LocationPostingDetailsDto> locationPostingDetailsDtoValidator)
    {
        // TODO: Figure out how to do a oneof for multiple properties at once.
        //       I wish C# supported discriminated unions natively.
        //       This is way too much boilerplate, but it's not like I could just
        //       easily substitute a generic property for them, or deduce the kind from json key name.
        RuleFor(x => x.RealEstate)
            .Null().When(x => x.Vehicle is not null)
            .WithMessage("Only one kind of product detail can be specified.");

        RuleFor(x => x.Vehicle)
            .Null().When(x => x.RealEstate is not null)
            .WithMessage("Only one kind of product detail can be specified.");

        RuleFor(x => x.RealEstate)
            .NotNull()
            .When(x => x.Kind == PostingKind.RealEstate);

        RuleFor(x => x.Vehicle)
            .NotNull()
            .When(x => x.Kind == PostingKind.Vehicle);
        
        RuleFor(x => x.Kind)
            .IsInEnum();
        
        // Apply the validators for each kind of detail
        RuleFor(x => x.RealEstate!)
            .SetValidator(realEstatePostingDetailsDtoValidator)
            .When(x => x.RealEstate is not null);
        
        RuleFor(x => x.Vehicle!)
            .SetValidator(vehiclePostingDetailsDtoValidator)
            .When(x => x.Vehicle is not null);
        
        RuleFor(x => x.Pricing!)
            .SetValidator(pricingPostingDetailsDtoValidator)
            .When(x => x.Pricing is not null);
        
        RuleFor(x => x.Location!)
            .SetValidator(locationPostingDetailsDtoValidator)
            .When(x => x.Location is not null);
    }
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
    [Required] public Guid Id { get; set; }
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

public sealed class PostingCreateDtoValidator : AbstractValidator<PostingCreateDto>
{
    public PostingCreateDtoValidator(IValidator<PostingDetailsDto> detailsValidator)
    {
        RuleFor(x => x.Title)
            .NotEmpty();
        RuleFor(x => x.Details)
            .SetValidator(detailsValidator);
    }
}

#pragma warning restore 8618 // Disable nullability warnings
