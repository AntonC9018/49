using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using fourtynine.DataAccess;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace fourtynine.Postings;

#pragma warning disable 8618 // Disable nullability warnings

public sealed class PostingGetDto_General
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
    [Required] public decimal? Price { get; set; }
    [Required] public decimal? PriceMax { get; set; }
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
    [Required] public string? City { get; set; }
    [Required] public string? Address { get; set; }
    [Required] public double? Latitude { get; set; }
    [Required] public double? Longitude { get; set; }
}

public sealed class PostingGetDto_Detailed
{
    public PostingGetDto_General General { get; set; }
    public List<string> PictureUrls { get; set; }
    public int AuthorId { get; set; }
    public PostingAuthorGetDto Author { get; set; }
    public PostingDetailsDto Details { get; set; }
}

public sealed class PostingAuthorGetDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public sealed class PostingCreateDto
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string ThumbnailUrl { get; set; }
    [Required]
    public PostingDetailsDto Details { get; set; }
}

#pragma warning restore 8618 // Disable nullability warnings

public struct Mapping<TFrom, TTo>
{
    public Expression<Func<TFrom, TTo>> Expression { get; }
    public Func<TFrom, TTo> Func { get; }
    public Mapping(Expression<Func<TFrom, TTo>> expression)
    {
        Expression = expression;
        Func = expression.Compile();
    }
    
    public static implicit operator Mapping<TFrom, TTo>(Expression<Func<TFrom, TTo>> expression)
        => new Mapping<TFrom, TTo>(expression);
}

public static partial class MappingExtensions
{
    public static Mapping<TFrom, TTo> To<TFrom, TTo>(this Expression<Func<TFrom, TTo>> expression)
    {
        return new Mapping<TFrom, TTo>(expression);
    }
}

public static partial class MappingExtensions
{
    // Yeah, I definitely need automapper, this is way too much boilerplate.
    
    private static readonly Mapping<PricingPostingDetails, PricingPostingDetailsDto> FromPricingDetailsMapping = new(
        x => new PricingPostingDetailsDto
        {
            BargainKinds = x.BargainKinds,
            Price = x.Price,
            PriceMax = x.PriceMax
        });

    private static readonly Mapping<LocationPostingDetails, LocationPostingDetailsDto> FromLocationDetailsMapping = new(
        x => new LocationPostingDetailsDto()
        {
            Country = x.Country,
            City = x.City,
            Address = x.Address,
            Latitude = x.Latitude,
            Longitude = x.Longitude
        });

    private static readonly Mapping<VehiclePostingDetails, VehiclePostingDetailsDto> FromVehicleDetailsMapping = new(
        x => new VehiclePostingDetailsDto
        {
            Year = x.Year,
            Manufacturer = x.Manufacturer,
            Model = x.Model,
        });

    private static readonly Mapping<RealEstatePostingDetails, RealEstatePostingDetailsDto> FromRealEstateDetailsMapping = new(
        x => new RealEstatePostingDetailsDto
        {
            Kind = x.Kind,
            SpacePurpose = x.SpacePurpose,
            Area = x.Area,
            Rooms = x.Rooms
        });
    
    private static readonly Mapping<PricingPostingDetailsDto, PricingPostingDetails> ToPricingDetailsMapping = new(
        x => new PricingPostingDetails
        {
            BargainKinds = x.BargainKinds,
            Price = x.Price,
            PriceMax = x.PriceMax
        });

    private static readonly Mapping<LocationPostingDetailsDto, LocationPostingDetails> ToLocationDetailsMapping = new(
        x => new LocationPostingDetails
        {
            Country = x.Country,
            City = x.City,
            Address = x.Address,
            Latitude = x.Latitude,
            Longitude = x.Longitude
        });

    private static readonly Mapping<VehiclePostingDetailsDto, VehiclePostingDetails> ToVehicleDetailsMapping = new(
        x => new VehiclePostingDetails
        {
            Year = x.Year,
            Manufacturer = x.Manufacturer,
            Model = x.Model,
        });

    private static readonly Mapping<RealEstatePostingDetailsDto, RealEstatePostingDetails> ToRealEstateDetailsMapping = new(
        x => new RealEstatePostingDetails
        {
            Kind = x.Kind,
            SpacePurpose = x.SpacePurpose,
            Area = x.Area,
            Rooms = x.Rooms
        });

    
    // TODO: Maybe do this with AutoMapper.
    private static readonly Mapping<Posting, PostingGetDto_Detailed> ToPostingMapping = new(
        x => new PostingGetDto_Detailed
        {
            General = new PostingGetDto_General
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                ThumbnailUrl = x.ThumbnailUrl,
                DatePosted = x.DatePosted,
            },
            PictureUrls = x.Pictures.Select(p => p.Url).ToList(),
            AuthorId = x.AuthorId,
            Author = new PostingAuthorGetDto
            {
                Id = x.Author.Id,
                Name = x.Author.Name,
                Email = x.Author.Email,
            },
            Details = new PostingDetailsDto
            {
                Pricing = ToPricingDetailsMapping.Expression.Expand(x),
                Vehicle = x.Vehicle,
                RealEstate = x.RealEstate,
                Location = x.Location,
            },
        });

    private static readonly Func<Posting, PostingGetDto_Detailed> _MapToGetDto_Detailed_Impl =
        _MapToGetDto_Detailed_Expr.Compile();


    public static IQueryable<PostingGetDto_General> MapToGetDto_General(this IQueryable<Posting> query)
    {
        return query.Select(x => new PostingGetDto_General
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            ThumbnailUrl = x.ThumbnailUrl,
            DatePosted = x.DatePosted,
        });
    }
    
    public static PostingGetDto_Detailed MapToGetDto_Detailed(this Posting posting)
    {
        return _MapToGetDto_Detailed_Impl(posting);
    }
    
    public static IQueryable<PostingGetDto_Detailed> MapToGetDto_Detailed(this IQueryable<Posting> query)
    {
        return query
            .Include(x => x.Author)
            .AsSplitQuery()
            .Include(x => x.Pictures)
            .Select(_MapToGetDto_Detailed_Expr.Expand());
    }
    
    public static Posting MapToEntity(this PostingCreateDto dto)
    {
        return new Posting
        {
            Title = dto.Title,
            Description = dto.Description,
            ThumbnailUrl = dto.ThumbnailUrl,
            Pricing = dto.Details.Pricing,
            Vehicle = dto.Details.Vehicle,
            RealEstate = dto.Details.RealEstate,
            Location = dto.Details.Location,
        };
    }
}
