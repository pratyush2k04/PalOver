using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Home;
using Pdr.Api.Exceptions;
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

        return user.Role.Name switch
        {
            "Project Management" => "/my-engagements",
            "Pre Sales" => "/my-handovers",
            "Tech Lead" => "/technical-readiness",
            "Leadership" => "/portfolio-dashboard",
            _ => throw new RoleNotFoundException(user.RoleId),
        };
    }

    private static string ResolveTitle(User user)
    {
        if (user.IsAdmin) return "User Directory";

        return user.Role.Name switch
        {
            "Project Management" => "My Engagements",
            "Pre Sales" => "My Handovers",
            "Tech Lead" => "Technical Readiness",
            "Leadership" => "Portfolio Dashboard",
            _ => throw new RoleNotFoundException(user.RoleId),
        };
    }

    private static string ResolveEmptyState(User user)
    {
        if (user.IsAdmin) return "No users found.";

        return user.Role.Name switch
        {
            "Project Management" => "You have no engagements yet.",
            "Pre Sales" => "You have no handovers yet.",
            "Tech Lead" => "You have no technical readiness tasks yet.",
            "Leadership" => "No portfolio items available.",
            _ => throw new RoleNotFoundException(user.RoleId),
        };
    }

    private static string BuildWelcomeMessage(User user)
    {
        var roleLabel = user.IsAdmin ? "Admin" : user.Role.Name;
        return $"Welcome, {user.FullName} - {roleLabel}";
    }
}
