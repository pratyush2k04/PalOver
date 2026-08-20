using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Search;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Provides permission-aware cross-entity search across engagements, documents, people, and checklist items.
/// </summary>
[ApiController]
[Authorize]
[Route("api/search")]
[Produces("application/json")]
public class SearchController(ISearchService searchService) : ControllerBase
{
    /// <summary>
    /// Searches across engagements, documents, people, checklist items, and decisions the current user can access.
    /// </summary>
    /// <param name="q">The search term.</param>
    /// <param name="types">Optional content types to filter by (e.g. engagement, document, person, checklist, decision).</param>
    /// <returns>Grouped search results with per-group counts.</returns>
    [HttpGet]
    [EndpointSummary("Global search across engagements, documents, people, checklist items, and decisions")]
    [ProducesResponseType(typeof(GlobalSearchResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GlobalSearchResponseDto>> GetSearch(
        [FromQuery] string? q,
        [FromQuery] string[]? types)
    {
        var result = await searchService.SearchAsync(q, types);
        return Ok(result);
    }
}
