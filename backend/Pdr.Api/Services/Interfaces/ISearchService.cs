using Pdr.Api.DTOs.Search;

namespace Pdr.Api.Services.Interfaces;

public interface ISearchService
{
    Task<GlobalSearchResponseDto> SearchAsync(string? query);
}
