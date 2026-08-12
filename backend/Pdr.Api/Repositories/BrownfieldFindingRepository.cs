using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.BrownField;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class BrownfieldFindingRepository(AppDbContext context) : IBrownfieldFindingRepository
{
    public async Task<List<BrownfieldFinding>> GetByEngagementIdAsync(int engagementId) =>
        await context.BrownfieldFindings
            .AsNoTracking()
            .Include(f => f.Document)
            .Where(f => f.EngagementId == engagementId)
            .OrderByDescending(f => f.Severity)
            .ThenByDescending(f => f.CreatedAt)
            .ToListAsync();

    public async Task<BrownfieldFinding?> GetByIdAsync(int findingId) =>
        await context.BrownfieldFindings
            .Include(f => f.Document)
            .FirstOrDefaultAsync(f => f.BrownfieldFindingId == findingId);

    public async Task AddAsync(BrownfieldFinding finding)
    {
        await context.BrownfieldFindings.AddAsync(finding);
    }

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();

    public async Task<bool> EngagementExistsAsync(int engagementId) =>
        await context.Engagements.AnyAsync(e => e.EngagementId == engagementId);

    public async Task<bool> DocumentExistsAsync(int documentId) =>
        await context.Documents.AnyAsync(d => d.Id == documentId);
}
