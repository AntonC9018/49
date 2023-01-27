using AutoMapper;
using AutoMapper.AspNet.OData;
using fourtynine.DataAccess;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using DbContext = fourtynine.DataAccess.DbContext;

namespace fourtynine.Postings;

public sealed class PostingApiService
{
    public DbContext DbContext { get; }
    public IMapper Mapper { get; }
    public IODataMapperProvider ODataMapperProvider { get; }
    
    public IMapper ODataMapper => ODataMapperProvider.Mapper;

    public PostingApiService(DbContext dbContext, IMapper mapper, IODataMapperProvider oDataMapperProvider)
    {
        DbContext = dbContext;
        Mapper = mapper;
        ODataMapperProvider = oDataMapperProvider;
    }
}

// The idea is that this thing will have a bunch more properties
// needed for the posting operations, like the mapper, validator, etc.
public static class PostingApiServiceExtensions
{
    public static async Task<PostingGetDto_General?> GetGeneral(
        this PostingApiService api, long id)
    {
        var dto = await api.DbContext.Postings
            .Where(p => p.Id == id)
            .ProjectTo<PostingGetDto_General>(api.Mapper)
            .FirstOrDefaultAsync();
        dto?.InitializeSlug();
        return dto;
    }
    
    public static IQueryable<T> ProjectTo<T>(this IQueryable query, IMapper mapper)
    {
        return mapper.ProjectTo<T>(query);
    }
    

    public static async Task<PostingGetDto_Detailed?> GetDetailed(
        this PostingApiService api, long id)
    {
        var dto = await api.DbContext.Postings
            .Where(p => p.Id == id)
            .ProjectTo<PostingGetDto_Detailed>(api.Mapper)
            .FirstOrDefaultAsync();
        dto?.General.InitializeSlug();
        return dto;
    }
    
    public static async Task<Posting> Create(
        this PostingApiService api, PostingCreateDto dto)
    {
        var posting = api.Mapper.Map<Posting>(dto);
        api.DbContext.Postings.Add(posting);
        await api.DbContext.SaveChangesAsync();
        return posting;
    }
    
    public static void InitializeSlug(this PostingGetDto_General dto)
    {
        dto.Slug = dto.Title.ToLower().Replace(' ', '-');
    }

    public static string GetSlug(this PostingGetDto_General dto)
    {
        if (dto.Slug is null)
            InitializeSlug(dto);
        return dto.Slug!;
    }
    
    private static readonly QuerySettings _GetQuerySettings = new QuerySettings
    {
        ODataSettings = new ODataSettings
        {
            PageSize = 100,
            HandleNullPropagation = HandleNullPropagationOption.Default,
        },
    };

    public static Task<ICollection<PostingGetDto_Detailed>> GetODataQuery(
        this PostingApiService api,
        ODataQueryOptions<PostingGetDto_Detailed> queryOptions)
    {
        return api.DbContext
            .Set<Posting>()
            .GetAsync(api.ODataMapper, queryOptions, _GetQuerySettings);
    }
}



#if false // could use this if the apis needed to be generalized.

public interface IApiBase
{
    public IMapper ODataMapper { get; }
    public IMapper Mapper { get; }
    public Microsoft.EntityFrameworkCore.DbContext DbContext { get; }
}

public interface IApiContext<TEntity, TDto> : IApiBase
    where TEntity : class
    where TDto : class
{
}

public static class ODataGenericExtensions
{
    public static Task<ICollection<TDto>> GetODataQuery<TEntity, TDto>(
        this IApiContext<TEntity, TDto> api,
        ODataQueryOptions<TDto> queryOptions)
        where TEntity : class
        where TDto : class
    {
        return api.DbContext
            .Set<TEntity>()
            .GetAsync(api.ODataMapper, queryOptions, _GetQuerySettings);
    }
}
    
#endif