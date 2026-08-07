using Microsoft.EntityFrameworkCore;
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
        if (user.IsAdmin) return "/user-directory";

        return (RoleEnum)user.RoleId switch
        {
            RoleEnum.ProjectManagement => "/engagement-dashboard",
            RoleEnum.PreSales => "/my-handovers",
            RoleEnum.TechLead => "/technical-readiness",
            RoleEnum.Leadership => "/portfolio-dashboard",
            _ => throw new RoleNotFoundException(user.RoleId),
        };
    }

    private static string ResolveTitle(User user)
    {
        if (user.IsAdmin) return "User Directory";

        return (RoleEnum)user.RoleId switch
        {
            RoleEnum.ProjectManagement => "My Engagements",
            RoleEnum.PreSales => "My Handovers",
            RoleEnum.TechLead => "Technical Readiness",
            RoleEnum.Leadership => "Portfolio Dashboard",
            _ => throw new RoleNotFoundException(user.RoleId),
        };
    }

    private static string ResolveEmptyState(User user)
    {
        if (user.IsAdmin) return "No users found.";

        return (RoleEnum)user.RoleId switch
        {
            RoleEnum.ProjectManagement => "You have no engagements yet.",
            RoleEnum.PreSales => "You have no handovers yet.",
            RoleEnum.TechLead => "You have no technical readiness tasks yet.",
            RoleEnum.Leadership => "No portfolio items available.",
            _ => throw new RoleNotFoundException(user.RoleId),
        };
    }

    private static string BuildWelcomeMessage(User user)
    {
        var roleLabel = user.IsAdmin ? "Admin" : user.Role.Name;
        return $"Welcome, {user.FullName} - {roleLabel}";
    }
}
