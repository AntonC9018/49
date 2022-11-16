using fourtynine.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fourtynine.Postings;

public class SearchQuery
{
    public int Count { get; set; } = 10;
    public long StartId { get; set; } = 0;
}

[ApiRoute]
[ApiControllerConvention]
public class PostingApiController : Controller
{
    private readonly PostingsDbContext _dbContext;
    
    public PostingApiController(PostingsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostingGetDto_General>> GetGeneral(long id)
    {
        var posting = await _dbContext.Postings
            .Where(p => p.Id == id)
            .MapToGetDto_General()
            .FirstOrDefaultAsync();
        if (posting is null)
            return NotFound();
        return posting;
    }
    
    [HttpGet("detailed/{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostingGetDto_Detailed>> GetDetailed(int id)
    {
        var posting = await _dbContext.Postings
            .Where(p => p.Id == id)
            .MapToGetDto_Detailed()
            .FirstOrDefaultAsync();
        if (posting is null)
            return NotFound();
        return posting;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<PostingGetDto_General>>> GetMany([FromQuery] SearchQuery query)
    {
        var postings = _dbContext.Postings
            .Where(p => p.Id >= query.StartId)
            .Take(query.Count);
        
        // Some more filtering here...
        // if (query.Blah)
        //      postings = postings.Where(Blah);

        return await postings.MapToGetDto_General().ToListAsync();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PostingGetDto_Detailed>> Create(PostingCreateDto postingDto)
    {
        var posting = postingDto.MapToEntity();
        _dbContext.Postings.Add(posting);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(
            nameof(GetDetailed),
            new { id = posting.Id }, 
            posting.MapToGetDto_Detailed());
    }
    
}