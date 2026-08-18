namespace Pdr.Api.DTOs.Risks;

public class RiskCommentDto
{
    public int Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsEdited { get; set; }
    public bool CanEdit { get; set; }
    public DateTime CreatedAt { get; set; }
}
