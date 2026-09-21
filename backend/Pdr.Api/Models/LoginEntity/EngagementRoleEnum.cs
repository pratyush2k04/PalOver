namespace Pdr.Api.Models.LoginEntity;

public enum EngagementRoleEnum
{
    PreSales = 1,
    Leadership = 2,
    ProjectManager = 3,
    TechLead = 4
}

public static class EngagementRoleEnumExtensions
{
    public static string ToRoleName(this EngagementRoleEnum role) => role switch
    {
        EngagementRoleEnum.PreSales => "Pre Sales",
        EngagementRoleEnum.Leadership => "Leadership",
        EngagementRoleEnum.ProjectManager => "Project Manager",
        EngagementRoleEnum.TechLead => "Tech Lead",
        _ => role.ToString()
    };
}
