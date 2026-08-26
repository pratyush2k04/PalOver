using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Notifications;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class KickoffReminderRepository(AppDbContext context) : IKickoffReminderRepository
{
    public async Task<List<Notification>> GetKickoffRemindersAsync(int engagementId, int templateId) =>
        await context.Notifications
            .IgnoreQueryFilters()
            .Where(n => n.TemplateId == templateId && EF.Functions.JsonContains(n.Data!, $"{{\"EngagementId\":{engagementId}}}"))
            .ToListAsync();
}
