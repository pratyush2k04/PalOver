namespace Pdr.Api.DTOs.Search;

public class GlobalSearchResponseDto
{
    public string Query { get; set; } = string.Empty;

    public int TotalCount { get; set; }

    public SearchEngagementGroupDto Engagements { get; set; } = new();

    public SearchDocumentGroupDto Documents { get; set; } = new();

    public SearchPersonGroupDto People { get; set; } = new();

    public SearchChecklistItemGroupDto ChecklistItems { get; set; } = new();
}

public class SearchEngagementGroupDto
{
    public int Count { get; set; }

    public List<SearchEngagementResultDto> Items { get; set; } = new();
}

public class SearchEngagementResultDto
{
    public int EngagementId { get; set; }

    public string EngagementName { get; set; } = string.Empty;

    public string ClientName { get; set; } = string.Empty;
}

public class SearchDocumentGroupDto
{
    public int Count { get; set; }

    public List<SearchDocumentResultDto> Items { get; set; } = new();
}

public class SearchDocumentResultDto
{
    public int DocumentId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public int EngagementId { get; set; }

    public string EngagementName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;
}

public class SearchPersonGroupDto
{
    public int Count { get; set; }

    public List<SearchPersonResultDto> Items { get; set; } = new();
}

public class SearchPersonResultDto
{
    public int PersonId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public int EngagementId { get; set; }

    public string EngagementName { get; set; } = string.Empty;
}

public class SearchChecklistItemGroupDto
{
    public int Count { get; set; }

    public List<SearchChecklistItemResultDto> Items { get; set; } = new();
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
