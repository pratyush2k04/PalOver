namespace Pdr.Api.Models.LoginEntity;

public enum UserRoleEnum
{
    PreSales = 1,
    Leadership = 2,
    Delivery = 3
}

public static class UserRoleEnumExtensions
{
    public static string ToRoleName(this UserRoleEnum role) => role switch
    {
        UserRoleEnum.PreSales => "Pre Sales",
        UserRoleEnum.Leadership => "Leadership",
        UserRoleEnum.Delivery => "Delivery",
        _ => role.ToString()
    };
}
