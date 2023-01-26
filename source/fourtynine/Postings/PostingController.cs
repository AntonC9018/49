using AutoMapper;
using AutoMapper.AspNet.OData;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using fourtynine.Authentication;
using fourtynine.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.EntityFrameworkCore;
using DbContext = fourtynine.DataAccess.DbContext;

namespace fourtynine.Postings;

public sealed class SearchQuery
{
    public int Count { get; set; } = 10;
    public long StartId { get; set; } = 0;
}

// The idea is that this thing will have a bunch more properties
// needed for the posting operations, like the mapper, validator, etc.
public sealed class PostingApiService
{
    public DbContext DbContext { get; }
    public IMapper Mapper { get; }

    public PostingApiService(DbContext dbContext, IMapper mapper)
    {
        DbContext = dbContext;
        Mapper = mapper;
    }
}
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
    
    public static async Task<List<PostingGetDto_General>> GetMany(
        this PostingApiService api, [FromQuery] SearchQuery query)
    {
        var postings = api.DbContext.Postings
            .OrderBy(p => p.Id)
            .Where(p => p.Id >= query.StartId)
            .Take(query.Count);
        
        // Some more filtering here...
        // if (query.Blah)
        //      postings = postings.Where(Blah);

        var list = await postings
            .ProjectTo<PostingGetDto_General>(api.Mapper)
            .ToListAsync();
        
        foreach (var dto in list)
            dto.InitializeSlug();

        return list;
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
}

[ApiRoute]
[ApiControllerConvention]
public class PostingController : Controller
{
    private readonly PostingApiService _api;
    private readonly ILogger _logger;
    
    public PostingController(PostingApiService api, ILogger<PostingController> logger)
    {
        _api = api;
        _logger = logger;
    }
    
    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostingGetDto_General>> GetGeneral(long id)
    {
        var posting = await _api.GetGeneral(id);
        if (posting is null)
            return NotFound();
        return posting;
    }
    
    [HttpGet("detailed/{id:long}", Name = nameof(GetDetailed))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostingGetDto_Detailed>> GetDetailed(int id)
    {
        var posting = await _api.GetDetailed(id);
        if (posting is null)
            return NotFound();
        return posting;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<PostingGetDto_General>>> GetMany([FromQuery] SearchQuery query)
    {
        return await _api.GetMany(query);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PostingGetDto_Detailed>> Create(
        PostingCreateDto postingDto, 
        [FromServices] IValidator<PostingCreateDto> validator)
    {
        _logger.LogInformation("Creating posting {Title}", postingDto.Title);

        var validationResult = await validator.ValidateAsync(postingDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        
        var posting = _api.Mapper.Map<Posting>(postingDto);
        posting.AuthorId = User.GetId();
        posting.DatePosted = DateTime.Now;

        _api.DbContext.Postings.Add(posting);
        await _api.DbContext.SaveChangesAsync();

        var dto = _api.Mapper.Map<PostingGetDto_Detailed>(posting);
        dto.General.InitializeSlug();
        
        return CreatedAtAction(nameof(GetDetailed), new { id = posting.Id }, dto);
    }
    
    private static readonly QuerySettings _QuerySettings = new QuerySettings
    {
        ODataSettings = new ODataSettings
        {
            PageSize = 100,
            HandleNullPropagation = HandleNullPropagationOption.Default,
        },
    };
    
    [HttpGet("odata")]
    [ODataAttributeRouting]
    public async Task<ActionResult<ICollection<PostingGetDto_Detailed>>> GetMany(ODataQueryOptions<PostingGetDto_Detailed> queryOptions)
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(ODataPostingMapperProfile.Instance);
        });
        var mapper = config.CreateMapper();
        
        var result = await _api.DbContext.Postings
            .GetAsync(mapper, queryOptions, _QuerySettings);
        return Ok(result);
    }
}