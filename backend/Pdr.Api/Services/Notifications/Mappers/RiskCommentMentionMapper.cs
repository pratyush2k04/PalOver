using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class RiskCommentMentionMapper(IRiskRepository riskRepository, IUserRepository userRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.RiskCommentMention.TemplateKey;

    public record RiskCommentMentionData(int EngagementId, int RiskId, int AuthorId, string Content);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<RiskCommentMentionData>(data)
            ?? throw new InvalidOperationException(NotificationConstants.RiskCommentMention.ParseErrorMessage);

        var risk = await riskRepository.GetByIdAsync(payload.RiskId);
        var author = await userRepository.GetByIdAsync(payload.AuthorId);

        var snippet = payload.Content.Length > 50
            ? payload.Content[..50] + "..."
            : payload.Content;

        return new Dictionary<string, object?>
        {
            [NotificationConstants.RiskCommentMention.RiskTitleKey] = risk?.Title ?? NotificationConstants.RiskCommentMention.DefaultRiskTitle,
            [NotificationConstants.RiskCommentMention.AuthorNameKey] = author?.FullName ?? NotificationConstants.RiskCommentMention.DefaultAuthorName,
            [NotificationConstants.RiskCommentMention.CommentSnippetKey] = snippet,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
