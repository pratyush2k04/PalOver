namespace Pdr.Api.Constants;

using Pdr.Api.Models.LoginEntity;

public static class RoleMappingConstants
{
    public const string InvalidMappingErrorMessageTemplate =
        "User with role '{0}' cannot be assigned to engagement role '{1}'. Allowed mappings are: Pre Sales -> Pre Sales, Leadership -> Leadership, Delivery -> Project Manager / Tech Lead.";

    public static readonly IReadOnlyDictionary<UserRoleEnum, IReadOnlyList<EngagementRoleEnum>> AllowedMappings =
        new Dictionary<UserRoleEnum, IReadOnlyList<EngagementRoleEnum>>
        {
            [UserRoleEnum.PreSales] = [EngagementRoleEnum.PreSales],
            [UserRoleEnum.Leadership] = [EngagementRoleEnum.Leadership],
            [UserRoleEnum.Delivery] = [EngagementRoleEnum.ProjectManager, EngagementRoleEnum.TechLead]
        };

    public static bool IsAllowedMapping(UserRoleEnum userRole, EngagementRoleEnum engagementRole)
    {
        return AllowedMappings.TryGetValue(userRole, out var allowed) && allowed.Contains(engagementRole);
    }

    public static void ValidateMapping(UserRoleEnum userRole, EngagementRoleEnum engagementRole)
    {
        if (!IsAllowedMapping(userRole, engagementRole))
        {
            var message = string.Format(
                InvalidMappingErrorMessageTemplate,
                userRole.ToRoleName(),
                engagementRole.ToRoleName());
            throw new InvalidOperationException(message);
        }
    }
}
