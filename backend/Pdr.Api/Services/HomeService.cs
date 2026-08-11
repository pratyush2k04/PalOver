using Microsoft.EntityFrameworkCore;
using Pdr.Api.Constants;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Home;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class HomeService(AppDbContext db) : IHomeService
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

        var homeDto = new HomeDashboardDto
        {
            Route = ResolveRoute(user),
            Title = ResolveTitle(user),
            WelcomeMessage = BuildWelcomeMessage(user),
            EmptyState = ResolveEmptyState(user),
        };

        return new HomeResponseDto { User = userDto, Home = homeDto };
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
            _ => throw new RoleNotFoundException(user.RoleId),
        };
    }

    private static string BuildWelcomeMessage(User user)
    {
        var roleLabel = user.IsAdmin ? RoleConstants.Admin : user.Role.Name;
        return $"Welcome, {user.FullName} - {roleLabel}";
    }
}
