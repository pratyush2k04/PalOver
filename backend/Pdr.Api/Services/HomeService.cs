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

public class HomeService(AppDbContext db, IKickoffReadinessService kickoffReadinessService) : IHomeService
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
            Role = user.Role.Name,
            IsAdmin = user.IsAdmin,
        };

        var myEngagementsQuery = db.EngagementAssignments
            .Where(ea => ea.UserId == user.Id)
            .Select(ea => ea.EngagementId);

        var cards = user.IsAdmin ? [] : await GetDashboardCardsAsync(user.Id, (RoleEnum)user.RoleId);
        var engagements = user.IsAdmin ? [] : await GetDashboardEngagementsAsync(myEngagementsQuery);
        var blockers = user.IsAdmin ? [] : await GetDashboardBlockersAsync(user.Id);

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
            new DashboardCardsDto { Title = "My Handovers", Count = myHandoversCount, Type = "engagement" },
            new DashboardCardsDto { Title = "Draft Handovers", Count = draftHandoversCount, Type = "handover" },
            new DashboardCardsDto { Title = "Completed Handovers", Count = completeHandoversCount, Type = "handover" }
        ];
    }

    private async Task<List<DashboardCardsDto>> GetProjectManagerCardsAsync(int userId, IQueryable<int> myEngagementsQuery)
    {
        var myEngagementsCount = await myEngagementsQuery.CountAsync();
        
        var checklistCount = await db.EngagementChecklistItems
            .Where(c => myEngagementsQuery.Contains(c.EngagementChecklist.EngagementId) 
                     && c.Status == "Incomplete" 
                     && c.IsMandatory)
            .CountAsync();
            
        var openBlockersCount = await db.Risks
            .Where(r => myEngagementsQuery.Contains(r.EngagementId) 
                     && r.Status == RiskStatus.Open)
            .CountAsync();

        return [
            new DashboardCardsDto { Title = "My Engagements", Count = myEngagementsCount, Type = "engagement" },
            new DashboardCardsDto { Title = "Checklist Actions", Count = checklistCount, Type = "checklist" },
            new DashboardCardsDto { Title = "Open Blockers", Count = openBlockersCount, Type = "blocker" }
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
            new DashboardCardsDto { Title = "My Tech Queue", Count = myTechQueueCount, Type = "tech-queue" },
            new DashboardCardsDto { Title = "Open Blockers", Count = openBlockersCount, Type = "blocker" },
            new DashboardCardsDto { Title = "Open Access Items", Count = openAccessItemsCount, Type = "access-item" }
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
            new DashboardCardsDto { Title = "My Engagements", Count = myEngagementsCount, Type = "engagement" },
            new DashboardCardsDto { Title = "Open Blockers", Count = openBlockersCount, Type = "blocker" },
            new DashboardCardsDto { Title = "My Escalations", Count = myEscalationsCount, Type = "escalation" }
        ];
    }

    private static string ResolveRoute(User user)
    {
        if (user.IsAdmin) return HomeConstants.FrontendRoutes.UserDirectory;

        return (RoleEnum)user.RoleId switch
        {
            RoleEnum.ProjectManagement => HomeConstants.FrontendRoutes.EngagementDashboard,
            RoleEnum.PreSales => HomeConstants.FrontendRoutes.MyHandovers,
            RoleEnum.TechLead => HomeConstants.FrontendRoutes.TechnicalReadiness,
            RoleEnum.Leadership => HomeConstants.FrontendRoutes.PortfolioDashboard,
            _ => throw new RoleNotFoundException(user.RoleId),
        };
    }

    private static string ResolveTitle(User user)
    {
        if (user.IsAdmin) return HomeConstants.Titles.UserDirectory;

        return (RoleEnum)user.RoleId switch
        {
            RoleEnum.ProjectManagement => HomeConstants.Titles.MyEngagements,
            RoleEnum.PreSales => HomeConstants.Titles.MyHandovers,
            RoleEnum.TechLead => HomeConstants.Titles.TechnicalReadiness,
            RoleEnum.Leadership => HomeConstants.Titles.PortfolioDashboard,
            _ => throw new RoleNotFoundException(user.RoleId),
        };
    }

    private static string ResolveEmptyState(User user)
    {
        if (user.IsAdmin) return HomeConstants.EmptyStates.UserDirectory;

        return (RoleEnum)user.RoleId switch
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
        var engagements = await db.Engagements
            .Where(e => myEngagementsQuery.Contains(e.EngagementId))
            .Select(e => new 
            {
                e.EngagementId,
                e.EngagementName,
                e.KickoffDate
            })
            .ToListAsync();

        var dtos = new List<DashboardEngagementDto>();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        foreach (var e in engagements)
        {
            var readiness = await kickoffReadinessService.GetReadinessAsync(e.EngagementId);
            var daysToKickoff = e.KickoffDate.DayNumber - today.DayNumber;

            int requiredThreshold;
            if (daysToKickoff > 14) requiredThreshold = LeadershipDashboardConstants.Thresholds.TierGreaterThan14;
            else if (daysToKickoff >= 8) requiredThreshold = LeadershipDashboardConstants.Thresholds.Tier8To14;
            else if (daysToKickoff >= 4) requiredThreshold = LeadershipDashboardConstants.Thresholds.Tier4To7;
            else if (daysToKickoff >= 1) requiredThreshold = LeadershipDashboardConstants.Thresholds.Tier1To3;
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
        var blockers = await db.Notifications
            .Where(n => n.ReceiverUserId == userId && n.Priority == NotificationPriority.High)
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .Select(n => new DashboardBlockerDto
            {
                RiskId = n.Id,
                ProjectName = n.Title ?? "Task",
                BlockerDescription = n.Body ?? "",
                OwnerName = "System",
                DueDate = n.ScheduledFor
            })
            .ToListAsync();
                              
        return blockers;
    }

    private static string BuildWelcomeMessage(User user)
    {
        var roleLabel = user.IsAdmin ? RoleConstants.Admin : user.Role.Name;
        return $"Welcome, {user.FullName} - {roleLabel}";
    }
}
