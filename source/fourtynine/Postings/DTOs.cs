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
    public PricingPostingDetails? Pricing { get; set; } 
    public VehiclePostingDetails? Vehicle { get; set; } 
    public RealEstatePostingDetails? RealEstate { get; set; } 
    public LocationPostingDetails? Location { get; set; } 
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
    public string Title { get; set; }
    public string Description { get; set; }
    public string ThumbnailUrl { get; set; }
    public PostingDetailsDto Details { get; set; }
}

#pragma warning restore 8618 // Disable nullability warnings

public static partial class MappingExtensions
{
    // TODO: Maybe do this with AutoMapper.
    private static readonly Expression<Func<Posting, PostingGetDto_Detailed>> _MapToGetDto_Detailed_Expr =
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
                Pricing = x.Pricing,
                Vehicle = x.Vehicle,
                RealEstate = x.RealEstate,
                Location = x.Location,
            },
        };

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
