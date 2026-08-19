namespace Pdr.Api.DTOs.Risks;

public class CreateRiskCommentDto
{
    public string Content { get; set; } = string.Empty;
    public List<int>? MentionedUserIds { get; set; }
}
