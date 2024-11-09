using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Helpers;
using SearchService.Models;

namespace SearchService.Controllers;

[ApiController]
[Route("[controller]")]

public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<GameItem>>> SearchItems([FromQuery]SearchParams searchParams)
    {
        var query = DB.PagedSearch<GameItem,GameItem>();
        if (!string.IsNullOrEmpty(searchParams.searchWord))
        {
        query.Match(Search.Full, searchParams.searchWord).SortByTextScore();
        }
        var result = await query.ExecuteAsync();
        return Ok(new{
            result = result.Results,
        });

    } 
}