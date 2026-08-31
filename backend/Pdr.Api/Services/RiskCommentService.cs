using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Risks;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Risks;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class RiskCommentService(
    IRiskCommentRepository commentRepository,
    IRiskRepository riskRepository,
    INotificationService notificationService) : IRiskCommentService
{
    public async Task<List<RiskCommentDto>> GetCommentsAsync(int engagementId, int riskId, int currentUserId)
    {
        var risk = await riskRepository.GetByIdAsync(riskId)
            ?? throw new RiskNotFoundException(riskId);

        if (risk.EngagementId != engagementId)
            throw new RiskNotFoundException(riskId);

        var comments = await commentRepository.GetCommentsByRiskIdAsync(riskId);

        return comments.Select(c => MapToDto(c, currentUserId)).ToList();
    }

    public async Task<RiskCommentDto> CreateCommentAsync(int engagementId, int riskId, CreateRiskCommentDto dto, int authorId)
    {
        if (await riskRepository.IsEngagementSignedOffAsync(engagementId))
        {
            throw new EngagementSignedOffException();
        }

        var risk = await riskRepository.GetByIdAsync(riskId)
            ?? throw new RiskNotFoundException(riskId);

        if (risk.EngagementId != engagementId)
            throw new RiskNotFoundException(riskId);

        var comment = new RiskComment
        {
            RiskId = riskId,
            AuthorId = authorId,
            Content = dto.Content,
            IsEdited = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await commentRepository.AddCommentAsync(comment);
        await commentRepository.SaveChangesAsync();

        var created = await commentRepository.GetCommentByIdAsync(comment.Id)
            ?? comment;

        // Process @mentions for users assigned to this engagement
        var members = await commentRepository.GetEngagementMembersAsync(engagementId);
        var targetMemberIds = new HashSet<int>();

        if (dto.MentionedUserIds != null && dto.MentionedUserIds.Count > 0)
        {
            var validIds = members.Select(m => m.Id).ToHashSet();
            foreach (var id in dto.MentionedUserIds)
            {
                if (validIds.Contains(id)) targetMemberIds.Add(id);
            }
        }

        foreach (var member in members)
        {
            if (dto.Content.Contains($"@{member.FullName}", StringComparison.OrdinalIgnoreCase) ||
                dto.Content.Contains($"@{member.Email.Split('@')[0]}", StringComparison.OrdinalIgnoreCase))
            {
                targetMemberIds.Add(member.Id);
            }
        }

        foreach (var receiverUserId in targetMemberIds.Where(id => id != authorId))
        {
            var payloadData = JsonSerializer.Serialize(new
            {
                EngagementId = engagementId,
                RiskId = riskId,
                AuthorId = authorId,
                Content = dto.Content
            });

            await notificationService.CreateFromTemplateAsync(
                NotificationConstants.RiskCommentMention.TemplateKey,
                receiverUserId,
                payloadData,
                DateTime.UtcNow);
        }

        return MapToDto(created, authorId);
    }

    public async Task<RiskCommentDto> UpdateCommentAsync(int engagementId, int riskId, int commentId, UpdateRiskCommentDto dto, int currentUserId)
    {
        if (await riskRepository.IsEngagementSignedOffAsync(engagementId))
        {
            throw new EngagementSignedOffException();
        }

        var risk = await riskRepository.GetByIdAsync(riskId)
            ?? throw new RiskNotFoundException(riskId);

        var comment = await commentRepository.GetCommentByIdAsync(commentId)
            ?? throw new RiskCommentNotFoundException(commentId);

        if (comment.RiskId != riskId || risk.EngagementId != engagementId)
            throw new RiskCommentNotFoundException(commentId);

        if (comment.AuthorId != currentUserId)
            throw new RiskCommentEditForbiddenException();

        if (DateTime.UtcNow - comment.CreatedAt > TimeSpan.FromMinutes(15))
            throw new RiskCommentEditWindowExpiredException();

        comment.Content = dto.Content;
        comment.IsEdited = true;
        comment.UpdatedAt = DateTime.UtcNow;

        await commentRepository.UpdateCommentAsync(comment);
        await commentRepository.SaveChangesAsync();

        return MapToDto(comment, currentUserId);
    }

    public async Task<List<EngagementMemberDto>> GetEngagementMembersAsync(int engagementId)
    {
        var members = await commentRepository.GetEngagementMembersAsync(engagementId);

        return members.Select(m => new EngagementMemberDto
        {
            Id = m.Id,
            FullName = m.FullName
        }).ToList();
    }

    private static RiskCommentDto MapToDto(RiskComment c, int currentUserId)
    {
        var canEdit = c.AuthorId == currentUserId && (DateTime.UtcNow - c.CreatedAt <= TimeSpan.FromMinutes(15));

        return new RiskCommentDto
        {
            Id = c.Id,
            AuthorName = c.Author?.FullName ?? "Unknown",
            Content = c.Content,
            IsEdited = c.IsEdited,
            CanEdit = canEdit,
            CreatedAt = c.CreatedAt
        };
    }
}
