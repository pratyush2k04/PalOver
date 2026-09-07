using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class RiskCommentMentionMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.RiskCommentMention.TemplateKey;

    public record RiskCommentMentionData(int EngagementId, int RiskId, int AuthorId, string Content);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<RiskCommentMentionData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.UserIds.Add(payload.AuthorId);
        context.RiskIds.Add(payload.RiskId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<RiskCommentMentionData>(data)
            ?? throw new InvalidOperationException(NotificationConstants.RiskCommentMention.ParseErrorMessage);

        var risk = context.Risks.GetValueOrDefault(payload.RiskId);
        var author = context.Users.GetValueOrDefault(payload.AuthorId);

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
