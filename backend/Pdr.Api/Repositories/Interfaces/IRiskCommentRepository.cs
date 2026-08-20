using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.Risks;

namespace Pdr.Api.Repositories.Interfaces;

public interface IRiskCommentRepository
{
    Task<List<RiskComment>> GetCommentsByRiskIdAsync(int riskId);
    Task<RiskComment?> GetCommentByIdAsync(int commentId);
    Task AddCommentAsync(RiskComment comment);
    Task UpdateCommentAsync(RiskComment comment);
    Task<List<User>> GetEngagementMembersAsync(int engagementId);
    Task SaveChangesAsync();
}
