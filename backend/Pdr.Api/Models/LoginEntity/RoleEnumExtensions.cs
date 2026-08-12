namespace Pdr.Api.Models.LoginEntity;

public static class RoleEnumExtensions
{
    public static string ToRoleName(this RoleEnum role) => role switch
    {
        RoleEnum.PreSales => "Pre Sales",
        RoleEnum.ProjectManagement => "Project Management",
        RoleEnum.TechLead => "Tech Lead",
        RoleEnum.Leadership => "Leadership",
        _ => role.ToString()
    };
}
