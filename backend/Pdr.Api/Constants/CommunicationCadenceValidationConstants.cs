namespace Pdr.Api.Constants;

public static class CommunicationCadenceValidationConstants
{
    public const int MaxStakeholderRoleLength = 200;
    public const string StakeholderRoleRequired = "Stakeholder role is required.";
    public const string StakeholderRoleMaxLength = "Stakeholder role cannot exceed 200 characters.";
    public const string CadenceInvalidEnum = "Cadence must be Daily, Weekly, Fortnightly, Monthly, or Quarterly.";
    public const string ParticipantUserIdsRequired = "At least one participant must be selected.";
}
