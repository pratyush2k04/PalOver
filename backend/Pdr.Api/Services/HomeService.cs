using Pdr.Api.Constants;
using Pdr.Api.DTOs.Common;
using Pdr.Api.DTOs.Home;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Models.Notifications;
using Pdr.Api.Helpers;

namespace Pdr.Api.Services;

public class HomeService(IHomeRepository homeRepository, IKickoffReadinessService kickoffReadinessService, INotificationService notificationService) : IHomeService
{
    public async Task<HomeResponseDto> GetHomeAsync(int userId)
    {
        var user = await homeRepository.GetUserWithRoleAsync(userId);

        if (user is null)
            throw new UserNotFoundException(userId);

        var userDto = new HomeUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Role = user.Role.Name
        };

        var myEngagementIds = await homeRepository.GetUserEngagementIdsAsync(user.Id);

        var effectiveRole = (RoleEnum)user.RoleId;

        var cards = await GetDashboardCardsAsync(user.Id, effectiveRole, myEngagementIds);
        var blockers = await GetDashboardBlockersAsync(user.Id);

        var homeDto = new HomeDashboardDto
        {
            Route = ResolveRoute(effectiveRole),
            Title = ResolveTitle(effectiveRole),
            WelcomeMessage = BuildWelcomeMessage(user),
            EmptyState = ResolveEmptyState(effectiveRole),
            Cards = cards,
            CriticalBlockers = blockers
        };

        return new HomeResponseDto { User = userDto, Home = homeDto };
    }

    private async Task<List<DashboardCardsDto>> GetDashboardCardsAsync(int userId, RoleEnum role, List<int> engagementIds)
    {
        return role switch
        {
            RoleEnum.PreSales => await GetPreSalesCardsAsync(engagementIds),
            RoleEnum.ProjectManagement => await GetProjectManagerCardsAsync(engagementIds),
            RoleEnum.TechLead => await GetTechLeadCardsAsync(engagementIds),
            RoleEnum.Leadership => await GetLeadershipCardsAsync(userId, engagementIds),
            _ => []
        };
    }

    private async Task<List<DashboardCardsDto>> GetPreSalesCardsAsync(List<int> engagementIds)
    {
        var myHandoversCount = engagementIds.Count;
        var draftHandoversCount = await homeRepository.GetDraftHandoversCountAsync(engagementIds);
        var completeHandoversCount = await homeRepository.GetCompleteHandoversCountAsync(engagementIds);

        return [
            new DashboardCardsDto { Title = HomeConstants.Titles.MyHandovers, Count = myHandoversCount, Type = HomeConstants.CardTypes.Engagement },
            new DashboardCardsDto { Title = "Draft Handovers", Count = draftHandoversCount, Type = HomeConstants.CardTypes.DraftHandover },
            new DashboardCardsDto { Title = "Completed Handovers", Count = completeHandoversCount, Type = HomeConstants.CardTypes.CompleteHandover }
        ];
    }

    private async Task<List<DashboardCardsDto>> GetProjectManagerCardsAsync(List<int> engagementIds)
    {
        var myEngagementsCount = engagementIds.Count;
        var checklistCount = await homeRepository.GetIncompleteChecklistCountAsync(engagementIds);
        var openBlockersCount = await homeRepository.GetOpenBlockersCountAsync(engagementIds);

        return [
            new DashboardCardsDto { Title = HomeConstants.Titles.MyEngagements, Count = myEngagementsCount, Type = HomeConstants.CardTypes.Engagement },
            new DashboardCardsDto { Title = "Checklist Actions", Count = checklistCount, Type = HomeConstants.CardTypes.Checklist },
            new DashboardCardsDto { Title = "Open Blockers", Count = openBlockersCount, Type = HomeConstants.CardTypes.Blocker }
        ];
    }

    private async Task<List<DashboardCardsDto>> GetTechLeadCardsAsync(List<int> engagementIds)
    {
        var myTechQueueCount = engagementIds.Count;
        var openBlockersCount = await homeRepository.GetOpenBlockersCountAsync(engagementIds);
        var openAccessItemsCount = await homeRepository.GetOpenAccessItemsCountAsync(engagementIds);

        return [
            new DashboardCardsDto { Title = "My Tech Queue", Count = myTechQueueCount, Type = HomeConstants.CardTypes.TechQueue },
            new DashboardCardsDto { Title = "Open Blockers", Count = openBlockersCount, Type = HomeConstants.CardTypes.Blocker },
            new DashboardCardsDto { Title = "Open Access Items", Count = openAccessItemsCount, Type = HomeConstants.CardTypes.AccessItem }
        ];
    }

    private async Task<List<DashboardCardsDto>> GetLeadershipCardsAsync(int userId, List<int> engagementIds)
    {
        var myEngagementsCount = engagementIds.Count;
        var openBlockersCount = await homeRepository.GetOpenBlockersCountAsync(engagementIds);
        var myEscalationsCount = await homeRepository.GetMyEscalationsCountAsync(userId);

        return [
            new DashboardCardsDto { Title = HomeConstants.Titles.MyEngagements, Count = myEngagementsCount, Type = HomeConstants.CardTypes.Engagement },
            new DashboardCardsDto { Title = "Open Blockers", Count = openBlockersCount, Type = HomeConstants.CardTypes.Blocker },
            new DashboardCardsDto { Title = "My Escalations", Count = myEscalationsCount, Type = HomeConstants.CardTypes.Escalation }
        ];
    }

    private static string ResolveRoute(RoleEnum effectiveRole)
    {
        return effectiveRole switch
        {
            RoleEnum.ProjectManagement => HomeConstants.FrontendRoutes.EngagementDashboard,
            RoleEnum.PreSales => HomeConstants.FrontendRoutes.MyHandovers,
            RoleEnum.TechLead => HomeConstants.FrontendRoutes.TechnicalReadiness,
            RoleEnum.Leadership => HomeConstants.FrontendRoutes.PortfolioDashboard,
            _ => throw new RoleNotFoundException((int)effectiveRole),
        };
    }

    private static string ResolveTitle(RoleEnum effectiveRole)
    {
        return effectiveRole switch
        {
            RoleEnum.ProjectManagement => HomeConstants.Titles.MyEngagements,
            RoleEnum.PreSales => HomeConstants.Titles.MyHandovers,
            RoleEnum.TechLead => HomeConstants.Titles.TechnicalReadiness,
            RoleEnum.Leadership => HomeConstants.Titles.PortfolioDashboard,
            _ => throw new RoleNotFoundException((int)effectiveRole),
        };
    }

    private static string ResolveEmptyState(RoleEnum effectiveRole)
    {
        return effectiveRole switch
        {
            RoleEnum.ProjectManagement => HomeConstants.EmptyStates.MyEngagements,
            RoleEnum.PreSales => HomeConstants.EmptyStates.MyHandovers,
            RoleEnum.TechLead => HomeConstants.EmptyStates.TechnicalReadiness,
            RoleEnum.Leadership => HomeConstants.EmptyStates.PortfolioDashboard,
            _ => string.Empty
        };
    }

    public async Task<PagedResult<DashboardEngagementDto>> GetDashboardEngagementsPageAsync(int userId, int page, int pageSize)
    {
        var myEngagementIds = await homeRepository.GetUserEngagementIdsAsync(userId);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var sevenDaysFromNow = today.AddDays(7);

        var (items, totalCount) = await homeRepository.GetUpcomingKickoffsPageAsync(myEngagementIds, sevenDaysFromNow, page, pageSize);

        foreach (var e in items)
        {
            var readiness = await kickoffReadinessService.GetReadinessAsync(e.EngagementId);
            var daysToKickoff = e.KickoffDate.DayNumber - today.DayNumber;

            int requiredThreshold = LeadershipDashboardHelpers.GetRequiredThreshold(daysToKickoff);
            int openBlockerCount = await homeRepository.GetEngagementOpenBlockerCountAsync(e.EngagementId);

            e.DaysToGo = daysToKickoff;
            e.ReadinessPercentage = readiness.Score;
            e.Status = LeadershipDashboardHelpers.DeriveStatus(readiness.Score, openBlockerCount, requiredThreshold);
        }

        return new PagedResult<DashboardEngagementDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    private async Task<List<DashboardBlockerDto>> GetDashboardBlockersAsync(int userId)
    {
        var result = await notificationService.GetForUserAsync(userId, null, 1, int.MaxValue);
        
        var blockers = result.Items
            .Where(n => n.Priority == NotificationPriority.High)
            .Select(n => new DashboardBlockerDto
            {
                RiskId = n.Id,
                ProjectName = n.Title ?? HomeConstants.Fallbacks.Task,
                DueDate = n.ReceivedAt
            })
            .ToList();
                              
        return blockers;
    }

    private static string BuildWelcomeMessage(User user)
    {
        var roleLabel = user.Role.Name;
        return $"Welcome, {user.FullName} - {roleLabel}";
    }
}
