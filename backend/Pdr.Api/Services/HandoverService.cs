using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Models.Handover;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class HandoverService : IHandoverService
{
    private readonly AppDbContext _context;

    public HandoverService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<HandoverInfoDto> GetHandoverAsync(long engagementId)
    {
        var handover = await _context.HandoverInfos
            .FirstOrDefaultAsync(h => h.EngagementId == engagementId);

        if (handover == null)
        {
            return new HandoverInfoDto
            {
                EngagementId = engagementId,
                Status = "Draft",
                Commitments = new List<CommitmentDto>()
            };
        }

        var status = await _context.HandoverStatuses
            .FirstOrDefaultAsync(s => s.HandoverStatusId == handover.StatusId);

        var commitments = await _context.Commitments
            .Where(c => c.HandoverId == handover.HandoverId)
            .Select(c => new CommitmentDto
            {
                CommitmentId = c.CommitmentId,
                HandoverId = c.HandoverId,
                Description = c.Description,
                CommitmentTypeId = c.CommitmentTypeId,
                Quantity = c.Quantity
            })
            .ToListAsync();

        return new HandoverInfoDto
        {
            HandoverId = handover.HandoverId,
            EngagementId = handover.EngagementId,
            Status = status?.StatusName ?? "Draft",
            ClientBackground = handover.ClientBackground,
            BusinessProblem = handover.BusinessProblem,
            ClientSentiment = handover.ClientSentiment,
            DealOrigin = handover.DealOrigin,
            HasNoCommitments = handover.HasNoCommitments,
            Commitments = commitments
        };
    }
}
