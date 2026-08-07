using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Exceptions;
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

    private async Task<HandoverInfo> GetHandoverOrThrowAsync(int engagementId)
    {
        var handover = await _context.HandoverInfos
            .FirstOrDefaultAsync(h => h.EngagementId == engagementId);

        if (handover == null)
            throw new HandoverNotFoundException(engagementId);

        return handover;
    }

    private async Task EnsureNotSubmittedAsync(HandoverInfo handover)
    {
        if (handover.StatusId == (int)HandoverStatusEnum.Submitted)
        {
            throw new HandoverSubmittedException("This handover has already been submitted and its commitments can no longer be changed.");
        }

        var status = await _context.HandoverStatuses
            .FirstOrDefaultAsync(s => s.HandoverStatusId == handover.StatusId);

        if (status?.StatusName == HandoverStatusNames.Submitted)
        {
            throw new HandoverSubmittedException("This handover has already been submitted and its commitments can no longer be changed.");
        }
    }

    public async Task<CommitmentDto> AddCommitmentAsync(int engagementId, CreateCommitmentRequest request, string userIdStr)
    {
        var handover = await GetHandoverOrThrowAsync(engagementId);
        await EnsureNotSubmittedAsync(handover);

        var commitment = new Commitment
        {
            HandoverId = handover.HandoverId,
            Description = request.Description,
            CommitmentTypeId = request.CommitmentTypeId,
            Quantity = request.Quantity,
            CreatedBy = userIdStr,
            UpdatedBy = userIdStr
        };

        _context.Commitments.Add(commitment);
        handover.HasNoCommitments = false;

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

    public async Task<CommitmentDto> UpdateCommitmentAsync(int engagementId, int commitmentId, UpdateCommitmentRequest request, string userIdStr)
    {
        var handover = await GetHandoverOrThrowAsync(engagementId);
        await EnsureNotSubmittedAsync(handover);

        var commitment = await _context.Commitments
            .FirstOrDefaultAsync(c => c.CommitmentId == commitmentId && c.HandoverId == handover.HandoverId);

        if (commitment == null) throw new CommitmentNotFoundException(commitmentId);

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

    public async Task DeleteCommitmentAsync(int engagementId, int commitmentId)
    {
        var handover = await GetHandoverOrThrowAsync(engagementId);
        await EnsureNotSubmittedAsync(handover);

        var commitment = await _context.Commitments
            .FirstOrDefaultAsync(c => c.CommitmentId == commitmentId && c.HandoverId == handover.HandoverId);

        if (commitment == null) throw new CommitmentNotFoundException(commitmentId);

        _context.Commitments.Remove(commitment);
        await _context.SaveChangesAsync();
    }
}
