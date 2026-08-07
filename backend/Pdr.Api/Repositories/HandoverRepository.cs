using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Handover;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class HandoverRepository(AppDbContext context) : IHandoverRepository
{
    public async Task<HandoverInfo?> GetByEngagementIdAsync(int engagementId) =>
        await context.HandoverInfos
            .FirstOrDefaultAsync(h => h.EngagementId == engagementId);

    public async Task AddAsync(HandoverInfo handover) =>
        await context.HandoverInfos.AddAsync(handover);

    public async Task<HandoverStatus?> GetStatusByIdAsync(int statusId) =>
        await context.HandoverStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.HandoverStatusId == statusId);

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();
}
