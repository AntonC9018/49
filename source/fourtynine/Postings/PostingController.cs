﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    public IMapper Mapper { get; }

    public PostingApiService(PostingsDbContext dbContext, IMapper mapper)
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
        var dto = _api.Mapper.Map<PostingGetDto_Detailed>(posting);
        dto.General.InitializeSlug();
        
        return CreatedAtAction(nameof(GetDetailed), new { id = posting.Id }, dto);
    }
    
}