using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class ClientContactTransitionRepository(AppDbContext context) : IClientContactTransitionRepository
{
    public async Task<ClientContactTransition?> GetByEngagementIdAsync(int engagementId)
    {
        return await context.ClientContactTransitions
            .Where(c => c.EngagementId == engagementId)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(ClientContactTransition transition)
    {
        await context.ClientContactTransitions.AddAsync(transition);
    }

    public void Update(ClientContactTransition transition)
    {
        context.ClientContactTransitions.Update(transition);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
