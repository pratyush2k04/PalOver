namespace Pdr.Api.Constants;

public static class CadenceConstants
{
    public static class Messages
    {
        public static string NotFound(int id) => $"Communication cadence with ID {id} was not found.";
        public static string StakeholderRoleAlreadyExists(string role) => $"A communication cadence for stakeholder role '{role}' already exists for this engagement.";
    }
}
