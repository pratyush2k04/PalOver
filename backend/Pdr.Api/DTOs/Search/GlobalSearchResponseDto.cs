namespace Pdr.Api.DTOs.Search;

public class GlobalSearchResponseDto
{
    public string Query { get; set; } = string.Empty;

    public int TotalCount { get; set; }

    public SearchResultGroupDto<SearchEngagementResultDto> Engagements { get; set; } = new();

    public SearchResultGroupDto<SearchDocumentResultDto> Documents { get; set; } = new();

    public SearchResultGroupDto<SearchPersonResultDto> People { get; set; } = new();

    public SearchResultGroupDto<SearchChecklistItemResultDto> ChecklistItems { get; set; } = new();

    public SearchResultGroupDto<SearchDecisionResultDto> Decisions { get; set; } = new();
}

public class SearchResultGroupDto<T>
{
    public int Count { get; set; }

    public List<T> Items { get; set; } = new();
}

public class SearchEngagementResultDto
{
    public int EngagementId { get; set; }

    public string EngagementName { get; set; } = string.Empty;

    public string ClientName { get; set; } = string.Empty;
}

public class SearchDocumentResultDto
{
    public int DocumentId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public int EngagementId { get; set; }

    public string EngagementName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;
}

public class SearchPersonResultDto
{
    public int PersonId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public int EngagementId { get; set; }

    public string EngagementName { get; set; } = string.Empty;
}

public class SearchChecklistItemResultDto
{
    public int ItemId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int EngagementId { get; set; }

    public string EngagementName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;
}

public class SearchDecisionResultDto
{
    public int DecisionId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int EngagementId { get; set; }

    public string EngagementName { get; set; } = string.Empty;
}
