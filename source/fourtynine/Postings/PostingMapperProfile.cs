using System.Reflection;
using AutoMapper;
using fourtynine.DataAccess;
using IProfileConfiguration = AutoMapper.IProfileConfiguration;

namespace fourtynine.Postings;

public abstract class PostingProfileBase : Profile
{
    protected new virtual IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>(
        MemberList memberList = MemberList.Destination)
    {
        return base.CreateMap<TSource, TDestination>(memberList);
    }

    private void AddMaps()
    {
        ClearPrefixes();

        CreateMap<ApplicationUser, PostingAuthorGetDto>(MemberList.Destination)
            .ForMember(d => d.Name,
                opt => opt.MapFrom(s => s.UserName));
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
    
    protected PostingProfileBase()
    {
        AddMaps();
    }
}

public sealed class PostingMapperProfile : PostingProfileBase
{
    public static readonly PostingMapperProfile Instance = new();

    public PostingMapperProfile() : base()
    {
    }
}

public sealed class ODataPostingMapperProfile : PostingProfileBase
{
    public static readonly ODataPostingMapperProfile Instance = new();
    
    // This way of solving it is a hack, and isn't scalable.
    // See a potential better impl below (doesn't work tho).
    protected override IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>(MemberList memberList = MemberList.Destination)
    {
        var m = base.CreateMap<TSource, TDestination>(memberList);
        m.ForAllMembers(o => o.ExplicitExpansion());
        return m;
    }

    public ODataPostingMapperProfile() : base()
    {
    }
}


#if false // doesn't work, don't know why, reflection sucks
public static class AutomapperProfileHax
{
    public static void ExplicitlyExpandAllMembersOfAllMaps(this IProfileConfiguration profile)
    {
        var expressionArgumentTypes = new System.Type[3];
        var argumentTypes = new System.Type[1];
        var parameters = new object?[1];
        
        foreach (var map in profile.TypeMapConfigs)
        {
            // void ForAllMembers(Action<IMemberConfigurationExpression<TSource, TDestination, object>> memberOptions);
            var mapType = map.GetType();
            
            var types = mapType.GetGenericArguments();
            expressionArgumentTypes[0] = types[0];
            expressionArgumentTypes[1] = types[1];
            expressionArgumentTypes[2] = typeof(object);
            
            var argumentType = typeof(IMemberConfigurationExpression<,,>).MakeGenericType(expressionArgumentTypes);
            argumentTypes[0] = argumentType;

            var actionType = typeof(Action<>).MakeGenericType(argumentTypes);
            argumentTypes[0] = actionType;
            
            var expand = _ExpandAllMembersMethodInfo.MakeGenericMethod(types).CreateDelegate(actionType);
            (map as dynamic).ForAllMembers(expand);

            // parameters[0] = expand;
            
            // var imappingExpressionMapType = typeof(IMappingExpression<,>).MakeGenericType(types);
            // This call doesn't find the method, whatever I do, no idea what's the problem.
            // var forAllMembers = imappingExpressionMapType.GetMethod("ForAllMembers", BindingFlags.Static | BindingFlags.Public, argumentTypes);
            // forAllMembers!.Invoke(map, parameters);
        } 
    }

    private static readonly MethodInfo _ExpandAllMembersMethodInfo = typeof(AutomapperProfileHax).GetMethod(nameof(ExpandAllMembers), BindingFlags.Static | BindingFlags.NonPublic)!;
    
    private static void ExpandAllMembers<TSource, TDestination>(IMemberConfigurationExpression<TSource, TDestination, object> configure)
    {
        configure.ExplicitExpansion();
    } 
}
#endif
