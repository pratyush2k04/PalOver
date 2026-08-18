using Pdr.Api.DTOs.Risks;

namespace Pdr.Api.Services.Interfaces;

public interface IRiskCommentService
{
    Task<List<RiskCommentDto>> GetCommentsAsync(int engagementId, int riskId, int currentUserId);
    Task<RiskCommentDto> CreateCommentAsync(int engagementId, int riskId, CreateRiskCommentDto dto, int authorId);
    Task<RiskCommentDto> UpdateCommentAsync(int engagementId, int riskId, int commentId, UpdateRiskCommentDto dto, int currentUserId);
    Task<List<EngagementMemberDto>> GetEngagementMembersAsync(int engagementId);
}
