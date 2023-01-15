using AutoMapper;
using fourtynine.DataAccess;

namespace fourtynine.Postings;

public class PostingMapperProfile : Profile
{
    public PostingMapperProfile()
    {
        ClearPrefixes();
        
        CreateMap<ApplicationUser, PostingAuthorGetDto>(MemberList.Destination);
        CreateMap<Posting, PostingGetDto_General>(MemberList.Destination)
            .ForMember(d => d.Slug, 
                opt => opt.Ignore());
        CreateMap<Posting, PostingGetDto_Detailed>(MemberList.Destination)
            .ForMember(d => d.General,
                opt => opt.MapFrom(s => s))
            .ForMember(d => d.PictureUrls,
                opt => opt.MapFrom(s => s.Pictures.Select(p => p.Url)))
            .ForMember(d => d.Details,
                opt => opt.MapFrom(s => s));

        CreateMap<Posting, PostingDetailsDto>(MemberList.Destination)
            .ReverseMap();
        CreateMap<PricingPostingDetails, PricingPostingDetailsDto>()
            .ReverseMap();
        CreateMap<LocationPostingDetails, LocationPostingDetailsDto>()
            .ReverseMap();
        CreateMap<RealEstatePostingDetails, RealEstatePostingDetailsDto>()
            .ReverseMap();
        CreateMap<VehiclePostingDetails, VehiclePostingDetailsDto>()
            .ReverseMap();

        CreateMap<PostingCreateDto, Posting>(MemberList.Source)
            .IncludeMembers(s => s.Details);
    }
}