namespace Pdr.Api.DTOs.Home;

public class HomeUserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
}

public class HomeDashboardDto
{
    public string Route { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string WelcomeMessage { get; set; } = string.Empty;
    public string EmptyState { get; set; } = string.Empty;
}

public class HomeResponseDto
{
    public HomeUserDto User { get; set; } = null!;
    public HomeDashboardDto Home { get; set; } = null!;
}
