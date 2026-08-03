using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Models.Handover;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class CommitmentService : ICommitmentService
{
    private readonly AppDbContext _context;

    public CommitmentService(AppDbContext context)
    {
        _context = context;
    }

    private async Task<long> EnsureHandoverDraftExistsAsync(long engagementId, string userIdStr)
    {
        var handover = await _context.HandoverInfos
            .FirstOrDefaultAsync(h => h.EngagementId == engagementId);

        if (handover == null)
        {
            handover = new HandoverInfo
            {
                EngagementId = engagementId,
                StatusId = 1,
                CreatedBy = userIdStr,
                UpdatedBy = userIdStr
            };
            _context.HandoverInfos.Add(handover);
            await _context.SaveChangesAsync();
        }

        return handover.HandoverId;
    }

    public async Task<CommitmentDto> AddCommitmentAsync(long engagementId, CreateCommitmentRequest request, string userIdStr)
    {
        var handoverId = await EnsureHandoverDraftExistsAsync(engagementId, userIdStr);

        var commitment = new Commitment
        {
            HandoverId = handoverId,
            Description = request.Description,
            CommitmentTypeId = request.CommitmentTypeId,
            Quantity = request.Quantity,
            CreatedBy = userIdStr,
            UpdatedBy = userIdStr
        };

        _context.Commitments.Add(commitment);

        var handover = await _context.HandoverInfos.FindAsync(handoverId);
        if (handover != null)
        {
            handover.HasNoCommitments = false;
        }

        await _context.SaveChangesAsync();

        return new CommitmentDto
        {
            CommitmentId = commitment.CommitmentId,
            HandoverId = commitment.HandoverId,
            Description = commitment.Description,
            CommitmentTypeId = commitment.CommitmentTypeId,
            Quantity = commitment.Quantity
        };
    }

    public async Task<CommitmentDto> UpdateCommitmentAsync(long engagementId, long commitmentId, UpdateCommitmentRequest request, string userIdStr)
    {
        var handover = await _context.HandoverInfos.FirstOrDefaultAsync(h => h.EngagementId == engagementId);
        if (handover == null) throw new System.Exception("Handover not found.");

        var commitment = await _context.Commitments
            .FirstOrDefaultAsync(c => c.CommitmentId == commitmentId && c.HandoverId == handover.HandoverId);

        if (commitment == null) throw new System.Exception("Commitment not found.");

        commitment.Description = request.Description;
        commitment.CommitmentTypeId = request.CommitmentTypeId;
        commitment.Quantity = request.Quantity;
        commitment.UpdatedBy = userIdStr;

        await _context.SaveChangesAsync();

        return new CommitmentDto
        {
            CommitmentId = commitment.CommitmentId,
            HandoverId = commitment.HandoverId,
            Description = commitment.Description,
            CommitmentTypeId = commitment.CommitmentTypeId,
            Quantity = commitment.Quantity
        };
    }

    public async Task DeleteCommitmentAsync(long engagementId, long commitmentId)
    {
        var handover = await _context.HandoverInfos.FirstOrDefaultAsync(h => h.EngagementId == engagementId);
        if (handover == null) throw new System.Exception("Handover not found.");

        var commitment = await _context.Commitments
            .FirstOrDefaultAsync(c => c.CommitmentId == commitmentId && c.HandoverId == handover.HandoverId);

        if (commitment == null) throw new System.Exception("Commitment not found.");

        _context.Commitments.Remove(commitment);
        await _context.SaveChangesAsync();
    }
}
