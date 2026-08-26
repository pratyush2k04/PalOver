using Microsoft.EntityFrameworkCore;
using Pdr.Api.Constants;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Home;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.Risks;
using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Models.Notifications;

namespace Pdr.Api.Services;

public class HomeService(AppDbContext db, IKickoffReadinessService kickoffReadinessService, INotificationService notificationService) : IHomeService
{
    public async Task<HomeResponseDto> GetHomeAsync(int userId)
    {
        var user = await db.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            throw new UserNotFoundException(userId);

        var userDto = new HomeUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Role = user.IsAdmin ? RoleConstants.Leadership : user.Role.Name,
            IsAdmin = user.IsAdmin,
        };

        var myEngagementsQuery = db.EngagementAssignments
            .Where(ea => ea.UserId == user.Id)
            .Select(ea => ea.EngagementId);

        var effectiveRole = user.IsAdmin ? RoleEnum.Leadership : (RoleEnum)user.RoleId;

        var cards = await GetDashboardCardsAsync(user.Id, effectiveRole);
        var engagements = await GetDashboardEngagementsAsync(myEngagementsQuery);
        var blockers = await GetDashboardBlockersAsync(user.Id);

        var homeDto = new HomeDashboardDto
        {
            Route = ResolveRoute(user),
            Title = ResolveTitle(user),
            WelcomeMessage = BuildWelcomeMessage(user),
            EmptyState = ResolveEmptyState(user),
            Cards = cards,
            Engagements = engagements,
            CriticalBlockers = blockers
        };

        return new HomeResponseDto { User = userDto, Home = homeDto };
    }

    private async Task<List<DashboardCardsDto>> GetDashboardCardsAsync(int userId, RoleEnum role)
    {
        var myEngagementsQuery = db.EngagementAssignments
            .Where(ea => ea.UserId == userId)
            .Select(ea => ea.EngagementId);

        return role switch
        {
            RoleEnum.PreSales => await GetPreSalesCardsAsync(myEngagementsQuery),
            RoleEnum.ProjectManagement => await GetProjectManagerCardsAsync(userId, myEngagementsQuery),
            RoleEnum.TechLead => await GetTechLeadCardsAsync(myEngagementsQuery),
            RoleEnum.Leadership => await GetLeadershipCardsAsync(userId, myEngagementsQuery),
            _ => []
        };
    }

    private async Task<List<DashboardCardsDto>> GetPreSalesCardsAsync(IQueryable<int> myEngagementsQuery)
    {
        var myHandoversCount = await myEngagementsQuery.CountAsync();
        
        var draftHandoversCount = await db.HandoverInfos
            .Where(h => myEngagementsQuery.Contains(h.EngagementId) 
                     && h.StatusId == (int)HandoverStatusEnum.Draft)
            .CountAsync();
            
        var completeHandoversCount = await db.HandoverInfos
            .Where(h => myEngagementsQuery.Contains(h.EngagementId) 
                     && h.StatusId == (int)HandoverStatusEnum.Submitted)
            .CountAsync();

        return [
            new DashboardCardsDto { Title = HomeConstants.Titles.MyHandovers, Count = myHandoversCount, Type = HomeConstants.CardTypes.Engagement },
            new DashboardCardsDto { Title = "Draft Handovers", Count = draftHandoversCount, Type = HomeConstants.CardTypes.DraftHandover },
            new DashboardCardsDto { Title = "Completed Handovers", Count = completeHandoversCount, Type = HomeConstants.CardTypes.CompleteHandover }
        ];
    }

    private async Task<List<DashboardCardsDto>> GetProjectManagerCardsAsync(int userId, IQueryable<int> myEngagementsQuery)
    {
        var myEngagementsCount = await myEngagementsQuery.CountAsync();
        
        var checklistCount = await db.EngagementChecklistItems
            .Where(c => myEngagementsQuery.Contains(c.EngagementChecklist.EngagementId) 
                     && c.Status == HomeConstants.Statuses.Incomplete 
                     && c.IsMandatory)
            .CountAsync();
            
        var openBlockersCount = await db.Risks
            .Where(r => myEngagementsQuery.Contains(r.EngagementId) 
                     && r.Status == RiskStatus.Open)
            .CountAsync();

        return [
            new DashboardCardsDto { Title = HomeConstants.Titles.MyEngagements, Count = myEngagementsCount, Type = HomeConstants.CardTypes.Engagement },
            new DashboardCardsDto { Title = "Checklist Actions", Count = checklistCount, Type = HomeConstants.CardTypes.Checklist },
            new DashboardCardsDto { Title = "Open Blockers", Count = openBlockersCount, Type = HomeConstants.CardTypes.Blocker }
        ];
    }

    private async Task<List<DashboardCardsDto>> GetTechLeadCardsAsync(IQueryable<int> myEngagementsQuery)
    {
        var myTechQueueCount = await myEngagementsQuery.CountAsync();
        
        var openBlockersCount = await db.Risks
            .Where(r => myEngagementsQuery.Contains(r.EngagementId) 
                     && r.Status == RiskStatus.Open)
            .CountAsync();
            
        var openAccessItemsCount = await db.AccessItems
            .Where(a => myEngagementsQuery.Contains(a.EngagementId) 
                     && a.Status == AccessItemStatus.InProgress)
            .CountAsync();

        return [
            new DashboardCardsDto { Title = "My Tech Queue", Count = myTechQueueCount, Type = HomeConstants.CardTypes.TechQueue },
            new DashboardCardsDto { Title = "Open Blockers", Count = openBlockersCount, Type = HomeConstants.CardTypes.Blocker },
            new DashboardCardsDto { Title = "Open Access Items", Count = openAccessItemsCount, Type = HomeConstants.CardTypes.AccessItem }
        ];
    }

    private async Task<List<DashboardCardsDto>> GetLeadershipCardsAsync(int userId, IQueryable<int> myEngagementsQuery)
    {
        var myEngagementsCount = await myEngagementsQuery.CountAsync();
        
        var openBlockersCount = await db.Risks
            .Where(r => myEngagementsQuery.Contains(r.EngagementId) 
                     && r.Status == RiskStatus.Open)
            .CountAsync();
            
        var myEscalationsCount = await db.RiskEscalationHistories
            .Where(e => e.EscalatedToUserId == userId)
            .CountAsync();

        return [
            new DashboardCardsDto { Title = HomeConstants.Titles.MyEngagements, Count = myEngagementsCount, Type = HomeConstants.CardTypes.Engagement },
            new DashboardCardsDto { Title = "Open Blockers", Count = openBlockersCount, Type = HomeConstants.CardTypes.Blocker },
            new DashboardCardsDto { Title = "My Escalations", Count = myEscalationsCount, Type = HomeConstants.CardTypes.Escalation }
        ];
    }

    private static string ResolveRoute(User user)
    {
        var effectiveRole = user.IsAdmin ? RoleEnum.Leadership : (RoleEnum)user.RoleId;

        return effectiveRole switch
        {
            RoleEnum.ProjectManagement => HomeConstants.FrontendRoutes.EngagementDashboard,
            RoleEnum.PreSales => HomeConstants.FrontendRoutes.MyHandovers,
            RoleEnum.TechLead => HomeConstants.FrontendRoutes.TechnicalReadiness,
            RoleEnum.Leadership => HomeConstants.FrontendRoutes.PortfolioDashboard,
            _ => throw new RoleNotFoundException((int)effectiveRole),
        };
    }

    private static string ResolveTitle(User user)
    {
        var effectiveRole = user.IsAdmin ? RoleEnum.Leadership : (RoleEnum)user.RoleId;

        return effectiveRole switch
        {
            RoleEnum.ProjectManagement => HomeConstants.Titles.MyEngagements,
            RoleEnum.PreSales => HomeConstants.Titles.MyHandovers,
            RoleEnum.TechLead => HomeConstants.Titles.TechnicalReadiness,
            RoleEnum.Leadership => HomeConstants.Titles.PortfolioDashboard,
            _ => throw new RoleNotFoundException((int)effectiveRole),
        };
    }

    private static string ResolveEmptyState(User user)
    {
        var effectiveRole = user.IsAdmin ? RoleEnum.Leadership : (RoleEnum)user.RoleId;

        return effectiveRole switch
        {
            RoleEnum.ProjectManagement => HomeConstants.EmptyStates.MyEngagements,
            RoleEnum.PreSales => HomeConstants.EmptyStates.MyHandovers,
            RoleEnum.TechLead => HomeConstants.EmptyStates.TechnicalReadiness,
            RoleEnum.Leadership => HomeConstants.EmptyStates.PortfolioDashboard,
            _ => string.Empty
        };
    }

    private async Task<List<DashboardEngagementDto>> GetDashboardEngagementsAsync(IQueryable<int> myEngagementsQuery)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var sevenDaysFromNow = today.AddDays(7);

        var engagements = await db.Engagements
            .Where(e => myEngagementsQuery.Contains(e.EngagementId) && e.KickoffDate <= sevenDaysFromNow)
            .OrderBy(e => e.KickoffDate)
            .Select(e => new 
            {
                e.EngagementId,
                e.EngagementName,
                e.KickoffDate
            })
            .ToListAsync();

        var dtos = new List<DashboardEngagementDto>();

        foreach (var e in engagements)
        {
            var readiness = await kickoffReadinessService.GetReadinessAsync(e.EngagementId);
            var daysToKickoff = e.KickoffDate.DayNumber - today.DayNumber;

            int requiredThreshold;
            if (daysToKickoff > LeadershipDashboardConstants.Days.TierGreaterThan14) requiredThreshold = LeadershipDashboardConstants.Thresholds.TierGreaterThan14;
            else if (daysToKickoff >= LeadershipDashboardConstants.Days.Tier8To14) requiredThreshold = LeadershipDashboardConstants.Thresholds.Tier8To14;
            else if (daysToKickoff >= LeadershipDashboardConstants.Days.Tier4To7) requiredThreshold = LeadershipDashboardConstants.Thresholds.Tier4To7;
            else if (daysToKickoff >= LeadershipDashboardConstants.Days.Tier1To3) requiredThreshold = LeadershipDashboardConstants.Thresholds.Tier1To3;
            else requiredThreshold = LeadershipDashboardConstants.Thresholds.TierDay0OrPast;

            int openBlockerCount = await db.Risks
                .CountAsync(r => r.EngagementId == e.EngagementId &&
                                 (r.Status == RiskStatus.Open || r.Status == RiskStatus.Mitigating) &&
                                 !r.IsDeleted);

            string status;
            if (readiness.Score == 100 && openBlockerCount == 0)
                status = LeadershipDashboardConstants.Statuses.KickoffReady;
            else if (openBlockerCount > 0 || readiness.Score < requiredThreshold)
                status = LeadershipDashboardConstants.Statuses.AtRisk;
            else
                status = LeadershipDashboardConstants.Statuses.InProgress;

            dtos.Add(new DashboardEngagementDto
            {
                EngagementId = e.EngagementId,
                EngagementName = e.EngagementName,
                KickoffDate = e.KickoffDate,
                DaysToGo = daysToKickoff,
                ReadinessPercentage = readiness.Score,
                Status = status
            });
        }

        return dtos;
    }

    private async Task<List<DashboardBlockerDto>> GetDashboardBlockersAsync(int userId)
    {
        var result = await notificationService.GetForUserAsync(userId, null, 1, 50);
        
        var blockers = result.Items
            .Where(n => n.Priority == NotificationPriority.High)
            .Take(5)
            .Select(n => new DashboardBlockerDto
            {
                RiskId = n.Id,
                ProjectName = n.Title ?? HomeConstants.Fallbacks.Task,
                BlockerDescription = n.Body ?? "",
                OwnerName = HomeConstants.Fallbacks.System,
                DueDate = n.ReceivedAt
            })
            .ToList();
                              
        return blockers;
    }

    private static string BuildWelcomeMessage(User user)
    {
        var roleLabel = user.IsAdmin ? RoleConstants.Leadership : user.Role.Name;
        return $"Welcome, {user.FullName} - {roleLabel}";
    }
}
