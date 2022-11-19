using fourtynine.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fourtynine.Postings;

public class SearchQuery
{
    public int Count { get; set; } = 10;
    public long StartId { get; set; } = 0;
}

// The idea is that this thing will have a bunch more properties
// needed for the posting operations, like the mapper, validator, etc.
public class PostingApiService
{
    public PostingsDbContext DbContext { get; }

    public PostingApiService(PostingsDbContext dbContext)
    {
        DbContext = dbContext;
    }
}
public static class PostingApiServiceExtensions
{
    public static Task<PostingGetDto_General?> GetGeneral(
        this PostingApiService api, long id)
    {
        return api.DbContext.Postings
            .Where(p => p.Id == id)
            .MapToGetDto_General()
            .FirstOrDefaultAsync();
    }

    public static Task<PostingGetDto_Detailed?> GetDetailed(
        this PostingApiService api, int id)
    {
        return api.DbContext.Postings
            .Where(p => p.Id == id)
            .MapToGetDto_Detailed()
            .FirstOrDefaultAsync();
    }
    
    public static async Task<Posting> Create(
        this PostingApiService api, PostingCreateDto dto)
    {
        var posting = dto.MapToEntity();
        api.DbContext.Postings.Add(posting);
        await api.DbContext.SaveChangesAsync();
        return posting;
    }
    
    public static async Task<List<PostingGetDto_General>> GetMany(
        this PostingApiService api, [FromQuery] SearchQuery query)
    {
        var postings = api.DbContext.Postings
            .Where(p => p.Id >= query.StartId)
            .Take(query.Count);
        
        // Some more filtering here...
        // if (query.Blah)
        //      postings = postings.Where(Blah);

        return await postings.MapToGetDto_General().ToListAsync();
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
    
    [HttpGet("detailed/{id:long}")]
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
    public async Task<ActionResult<PostingGetDto_Detailed>> Create(PostingCreateDto postingDto)
    {
        _logger.LogInformation("Creating posting {title}", postingDto.Title);
        
        var posting = await _api.Create(postingDto);
        return CreatedAtAction(
            nameof(GetDetailed),
            new { id = posting.Id }, 
            posting.MapToGetDto_Detailed());
    }
    
}