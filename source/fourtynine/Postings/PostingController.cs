using FluentValidation;
using fourtynine.Authentication;
using fourtynine.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;

namespace fourtynine.Postings;

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
    
    [HttpGet("odata")]
    [ODataAttributeRouting]
    public async Task<ActionResult<ICollection<PostingGetDto_Detailed>>> Get(
        ODataQueryOptions<PostingGetDto_Detailed> queryOptions)
    {
        return Ok(await _api.GetODataQuery(queryOptions));
    }
}