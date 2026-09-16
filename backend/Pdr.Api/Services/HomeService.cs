using Pdr.Api.Constants;
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

        var cards = await GetDashboardCardsAsync(user.Id, (UserRoleEnum)user.RoleId, myEngagementIds);
        var (upcomingEngagements, totalUpcomingEngagements) = await GetUpcomingEngagementsAsync(user.Id, myEngagementIds);
        var (blockers, totalBlockers) = await GetDashboardBlockersAsync(user.Id);

        var homeDto = new HomeDashboardDto
        {
            Cards = cards,
            UpcomingEngagements = upcomingEngagements,
            TotalUpcomingEngagements = totalUpcomingEngagements,
            CriticalBlockers = blockers,
            TotalCriticalBlockers = totalBlockers
        };

        return new HomeResponseDto { User = userDto, Home = homeDto };
    }

    private async Task<List<DashboardCardsDto>> GetDashboardCardsAsync(int userId, UserRoleEnum role, List<int> engagementIds)
    {
        return role switch
        {
            UserRoleEnum.PreSales => await GetPreSalesCardsAsync(engagementIds),
            UserRoleEnum.Leadership => await GetLeadershipCardsAsync(userId, engagementIds),
            UserRoleEnum.Delivery => await GetDeliveryCardsAsync(userId, engagementIds),
            _ => []
        };
    }

    private async Task<List<DashboardCardsDto>> GetDeliveryCardsAsync(int userId, List<int> engagementIds)
    {
        var myEngagementsCount = engagementIds.Count;

        var pmEngagementIds = await homeRepository.GetUserEngagementIdsByRoleAsync(userId, (int)EngagementRoleEnum.ProjectManager);
        var checklistCount = await homeRepository.GetIncompleteChecklistCountAsync(pmEngagementIds);

        var tlEngagementIds = await homeRepository.GetUserEngagementIdsByRoleAsync(userId, (int)EngagementRoleEnum.TechLead);
        var openAccessItemsCount = await homeRepository.GetOpenAccessItemsCountAsync(tlEngagementIds);

        var openBlockersCount = await homeRepository.GetOpenBlockersCountAsync(engagementIds);

        return [
            new DashboardCardsDto { Type = HomeConstants.CardTypes.Engagement, Count = myEngagementsCount },
            new DashboardCardsDto { Type = HomeConstants.CardTypes.Checklist, Count = checklistCount },
            new DashboardCardsDto { Type = HomeConstants.CardTypes.AccessItem, Count = openAccessItemsCount },
            new DashboardCardsDto { Type = HomeConstants.CardTypes.Blocker, Count = openBlockersCount }
        ];
    }

    private async Task<List<DashboardCardsDto>> GetPreSalesCardsAsync(List<int> engagementIds)
    {
        var myHandoversCount = engagementIds.Count;
        var draftHandoversCount = await homeRepository.GetDraftHandoversCountAsync(engagementIds);
        var completeHandoversCount = await homeRepository.GetCompleteHandoversCountAsync(engagementIds);

        return [
            new DashboardCardsDto { Type = HomeConstants.CardTypes.Engagement, Count = myHandoversCount },
            new DashboardCardsDto { Type = HomeConstants.CardTypes.DraftHandover, Count = draftHandoversCount },
            new DashboardCardsDto { Type = HomeConstants.CardTypes.CompleteHandover, Count = completeHandoversCount }
        ];
    }

    private async Task<List<DashboardCardsDto>> GetProjectManagerCardsAsync(List<int> engagementIds)
    {
        var myEngagementsCount = engagementIds.Count;
        var checklistCount = await homeRepository.GetIncompleteChecklistCountAsync(engagementIds);
        var openBlockersCount = await homeRepository.GetOpenBlockersCountAsync(engagementIds);

        return [
            new DashboardCardsDto { Type = HomeConstants.CardTypes.Engagement, Count = myEngagementsCount },
            new DashboardCardsDto { Type = HomeConstants.CardTypes.Checklist, Count = checklistCount },
            new DashboardCardsDto { Type = HomeConstants.CardTypes.Blocker, Count = openBlockersCount }
        ];
    }

    private async Task<List<DashboardCardsDto>> GetTechLeadCardsAsync(List<int> engagementIds)
    {
        var myTechQueueCount = engagementIds.Count;
        var openBlockersCount = await homeRepository.GetOpenBlockersCountAsync(engagementIds);
        var openAccessItemsCount = await homeRepository.GetOpenAccessItemsCountAsync(engagementIds);

        return [
            new DashboardCardsDto { Type = HomeConstants.CardTypes.TechQueue, Count = myTechQueueCount },
            new DashboardCardsDto { Type = HomeConstants.CardTypes.Blocker, Count = openBlockersCount },
            new DashboardCardsDto { Type = HomeConstants.CardTypes.AccessItem, Count = openAccessItemsCount }
        ];
    }

    private async Task<List<DashboardCardsDto>> GetLeadershipCardsAsync(int userId, List<int> engagementIds)
    {
        var myEngagementsCount = engagementIds.Count;
        var openBlockersCount = await homeRepository.GetOpenBlockersCountAsync(engagementIds);
        var myEscalationsCount = await homeRepository.GetMyEscalationsCountAsync(userId);

        return [
            new DashboardCardsDto { Type = HomeConstants.CardTypes.Engagement, Count = myEngagementsCount },
            new DashboardCardsDto { Type = HomeConstants.CardTypes.Blocker, Count = openBlockersCount },
            new DashboardCardsDto { Type = HomeConstants.CardTypes.Escalation, Count = myEscalationsCount }
        ];
    }

    private async Task<(List<DashboardEngagementDto> Items, int TotalCount)> GetUpcomingEngagementsAsync(int userId, List<int> engagementIds)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var sevenDaysFromNow = today.AddDays(7);

        var (items, totalCount) = await homeRepository.GetTopUpcomingKickoffsAsync(userId, engagementIds, sevenDaysFromNow, take: 10);

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

        return (items, totalCount);
    }

    private async Task<(List<DashboardBlockerDto> Items, int TotalCount)> GetDashboardBlockersAsync(int userId)
    {
        var result = await notificationService.GetForUserAsync(userId, null, 1, 5, isRead: false, priority: NotificationPriority.High);

        var blockers = result.Items
            .Select(n => new DashboardBlockerDto
            {
                RiskId = n.Id,
                ProjectName = n.Title ?? HomeConstants.Fallbacks.Task,
                DueDate = n.ReceivedAt,
                Link = n.Link
            })
            .ToList();

        return (blockers, result.TotalCount);
    }
}
