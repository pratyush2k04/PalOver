namespace Pdr.Api.Constants;

public static class AuditConstants
{
    public static class AccessItemBlocked
    {
        public const string TemplateKey = "AccessItemBlocked";
        public const string ParseErrorMessage = "AccessItemBlocked audit payload could not be parsed.";
        public const string ItemNameKey = "itemName";
        public const string EngagementNameKey = "engagementName";
        public const string ActorNameKey = "actorName";
        public const string DefaultAccessitem = "Access item";
        public const string DefaultEngagement = "Engagement";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class AccountLockout
    {
        public const string TemplateKey = "AccountLockout";
        public const string ParseErrorMessage = "AccountLockout audit payload could not be parsed.";
        public const string UserNameKey = "userName";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class AssumptionCreated
    {
        public const string TemplateKey = "AssumptionCreated";
        public const string ParseErrorMessage = "AssumptionCreated audit payload could not be parsed.";
        public const string AssumptionDescriptionKey = "assumptionDescription";
        public const string EngagementNameKey = "engagementName";
        public const string DefaultEngagement = "Engagement";
    }

    public static class AssumptionUpdated
    {
        public const string TemplateKey = "AssumptionUpdated";
        public const string ParseErrorMessage = "AssumptionUpdated audit payload could not be parsed.";
        public const string AssumptionDescriptionKey = "assumptionDescription";
        public const string EngagementNameKey = "engagementName";
        public const string DefaultEngagement = "Engagement";
    }

    public static class AssumptionValidated
    {
        public const string TemplateKey = "AssumptionValidated";
        public const string ParseErrorMessage = "AssumptionValidated audit payload could not be parsed.";
        public const string AssumptionDescriptionKey = "assumptionDescription";
        public const string AssumptionStatusKey = "assumptionStatus";
        public const string EngagementNameKey = "engagementName";
        public const string DefaultEngagement = "Engagement";
    }

    public static class BrownfieldItemAdded
    {
        public const string TemplateKey = "BrownfieldItemAdded";
        public const string ParseErrorMessage = "BrownfieldItemAdded audit payload could not be parsed.";
        public const string FindingNameKey = "findingName";
        public const string EngagementNameKey = "engagementName";
        public const string ActorNameKey = "actorName";
        public const string DefaultFinding = "Finding";
        public const string DefaultEngagement = "Engagement";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class ChecklistItemEvidenceUpdated
    {
        public const string TemplateKey = "ChecklistItemEvidenceUpdated";
        public const string ParseErrorMessage = "ChecklistItemEvidenceUpdated audit payload could not be parsed.";
        public const string ItemTitleKey = "itemTitle";
        public const string EngagementNameKey = "engagementName";
        public const string ActorNameKey = "actorName";
        public const string DefaultChecklistItem = "Checklist Item";
        public const string DefaultEngagement = "Engagement";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class ClientContactDetailsChanged
    {
        public const string TemplateKey = "ClientContactDetailsChanged";
        public const string ParseErrorMessage = "ClientContactDetailsChanged audit payload could not be parsed.";
        public const string ClientNameKey = "clientName";
        public const string ActorNameKey = "actorName";
        public const string DefaultClient = "Client";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class ClientContactTransitioned
    {
        public const string TemplateKey = "ClientContactTransitioned";
        public const string ParseErrorMessage = "ClientContactTransitioned audit payload could not be parsed.";
        public const string OutgoingNameKey = "outgoingName";
        public const string IncomingNameKey = "incomingName";
        public const string ActorNameKey = "actorName";
        public const string DefaultUnknown = "Unknown";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class ClientPersonaChanged
    {
        public const string TemplateKey = "ClientPersonaChanged";
        public const string ParseErrorMessage = "ClientPersonaChanged audit payload could not be parsed.";
        public const string ClientNameKey = "clientName";
        public const string ActorNameKey = "actorName";
        public const string DefaultClient = "Client";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class DayOneSummaryGenerated
    {
        public const string TemplateKey = "DayOneSummaryGenerated";
        public const string ParseErrorMessage = "DayOneSummaryGenerated audit payload could not be parsed.";
        public const string EngagementNameKey = "engagementName";
        public const string ActorNameKey = "actorName";
        public const string DefaultEngagement = "Engagement";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class DependencyCreated
    {
        public const string TemplateKey = "DependencyCreated";
        public const string ParseErrorMessage = "DependencyCreated audit payload could not be parsed.";
        public const string DependencyDescriptionKey = "dependencyDescription";
        public const string DependencyTypeKey = "dependencyType";
        public const string EngagementNameKey = "engagementName";
        public const string DefaultEngagement = "Engagement";
    }

    public static class DependencyDeleted
    {
        public const string TemplateKey = "DependencyDeleted";
        public const string ParseErrorMessage = "DependencyDeleted audit payload could not be parsed.";
        public const string DependencyDescriptionKey = "dependencyDescription";
        public const string EngagementNameKey = "engagementName";
        public const string DefaultEngagement = "Engagement";
    }

    public static class DependencyEscalated
    {
        public const string TemplateKey = "DependencyEscalated";
        public const string ParseErrorMessage = "DependencyEscalated audit payload could not be parsed.";
        public const string DependencyDescriptionKey = "dependencyDescription";
        public const string RiskTitleKey = "riskTitle";
        public const string EngagementNameKey = "engagementName";
        public const string DefaultEscalatedRisk = "Escalated Risk";
        public const string DefaultEngagement = "Engagement";
    }

    public static class DependencyUpdated
    {
        public const string TemplateKey = "DependencyUpdated";
        public const string ParseErrorMessage = "DependencyUpdated audit payload could not be parsed.";
        public const string DependencyDescriptionKey = "dependencyDescription";
        public const string DependencyStatusKey = "dependencyStatus";
        public const string EngagementNameKey = "engagementName";
        public const string DefaultEngagement = "Engagement";
    }

    public static class DiscoveryItemAdded
    {
        public const string TemplateKey = "DiscoveryItemAdded";
        public const string ParseErrorMessage = "DiscoveryItemAdded audit payload could not be parsed.";
        public const string ItemNameKey = "itemName";
        public const string EngagementNameKey = "engagementName";
        public const string ActorNameKey = "actorName";
        public const string DefaultItem = "Item";
        public const string DefaultEngagement = "Engagement";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class DocumentDeleted
    {
        public const string TemplateKey = "DocumentDeleted";
        public const string ParseErrorMessage = "DocumentDeleted audit payload could not be parsed.";
        public const string DocumentNameKey = "documentName";
        public const string ReasonKey = "reason";
        public const string ActorNameKey = "actorName";
        public const string DefaultUnknownDocument = "Unknown Document";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class DocumentUploaded
    {
        public const string TemplateKey = "DocumentUploaded";
        public const string ParseErrorMessage = "DocumentUploaded audit payload could not be parsed.";
        public const string DocumentNameKey = "documentName";
        public const string CategoryNameKey = "categoryName";
        public const string ActorNameKey = "actorName";
        public const string DefaultUnknownDocument = "Unknown Document";
        public const string DefaultUnknownCategory = "Unknown Category";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class DocumentVersioned
    {
        public const string TemplateKey = "DocumentVersioned";
        public const string ParseErrorMessage = "DocumentVersioned audit payload could not be parsed.";
        public const string DocumentNameKey = "documentName";
        public const string VersionKey = "version";
        public const string ActorNameKey = "actorName";
        public const string DefaultUnknownDocument = "Unknown Document";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class DocumentVisibilityChanged
    {
        public const string TemplateKey = "DocumentVisibilityChanged";
        public const string ParseErrorMessage = "DocumentVisibilityChanged audit payload could not be parsed.";
        public const string DocumentNameKey = "documentName";
        public const string VisibilityKey = "visibility";
        public const string ActorNameKey = "actorName";
        public const string DefaultUnknownDocument = "Unknown Document";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class HandoverQueryRaised
    {
        public const string TemplateKey = "HandoverQueryRaised";
        public const string ParseErrorMessage = "HandoverQueryRaised audit payload could not be parsed.";
        public const string ActorNameKey = "actorName";
        public const string FieldSectionKey = "fieldSection";
        public const string EngagementNameKey = "engagementName";
    }

    public static class HandoverQueryResolved
    {
        public const string TemplateKey = "HandoverQueryResolved";
        public const string ParseErrorMessage = "HandoverQueryResolved audit payload could not be parsed.";
        public const string ActorNameKey = "actorName";
        public const string FieldSectionKey = "fieldSection";
        public const string EngagementNameKey = "engagementName";
    }

    public static class HandoverQueryResponded
    {
        public const string TemplateKey = "HandoverQueryResponded";
        public const string ParseErrorMessage = "HandoverQueryResponded audit payload could not be parsed.";
        public const string ActorNameKey = "actorName";
        public const string FieldSectionKey = "fieldSection";
        public const string EngagementNameKey = "engagementName";
    }

    public static class KickoffReadyMarked
    {
        public const string TemplateKey = "KickoffReadyMarked";
        public const string ParseErrorMessage = "KickoffReadyMarked audit payload could not be parsed.";
        public const string EngagementNameKey = "engagementName";
        public const string ActorNameKey = "actorName";
        public const string DefaultEngagement = "Engagement";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class KickoffReadyReverted
    {
        public const string TemplateKey = "KickoffReadyReverted";
        public const string ParseErrorMessage = "KickoffReadyReverted audit payload could not be parsed.";
        public const string EngagementNameKey = "engagementName";
        public const string ActorNameKey = "actorName";
        public const string DefaultEngagement = "Engagement";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class LeadershipAllocated
    {
        public const string TemplateKey = "LeadershipAllocated";
        public const string ParseErrorMessage = "LeadershipAllocated audit payload could not be parsed.";
        public const string EngagementNameKey = "engagementName";
        public const string ActorNameKey = "actorName";
        public const string DefaultEngagement = "Engagement";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class MasterChecklistPublished
    {
        public const string TemplateKey = "MasterChecklistPublished";
        public const string ParseErrorMessage = "MasterChecklistPublished audit payload could not be parsed.";
        public const string VersionKey = "version";
        public const string ActorNameKey = "actorName";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class PersonAdded
    {
        public const string TemplateKey = "PersonAdded";
        public const string ParseErrorMessage = "PersonAdded audit payload could not be parsed.";
        public const string PersonNameKey = "personName";
        public const string RoleNameKey = "roleName";
        public const string ActorNameKey = "actorName";
    }

    public static class PmAndTlAssigned
    {
        public const string TemplateKey = "PmAndTlAssigned";
        public const string ParseErrorMessage = "PmAndTlAssigned audit payload could not be parsed.";
        public const string EngagementNameKey = "engagementName";
        public const string ActorNameKey = "actorName";
        public const string DefaultEngagement = "Engagement";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class PmAndTlReassigned
    {
        public const string TemplateKey = "PmAndTlReassigned";
        public const string ParseErrorMessage = "PmAndTlReassigned audit payload could not be parsed.";
        public const string EngagementNameKey = "engagementName";
        public const string ActorNameKey = "actorName";
        public const string DefaultEngagement = "Engagement";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class ReadinessOnePagerGenerated
    {
        public const string TemplateKey = "ReadinessOnePagerGenerated";
        public const string ParseErrorMessage = "ReadinessOnePagerGenerated audit payload could not be parsed.";
        public const string EngagementNameKey = "engagementName";
        public const string ActorNameKey = "actorName";
        public const string DefaultEngagement = "Engagement";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class ResourcingGapFlagCleared
    {
        public const string TemplateKey = "ResourcingGapFlagCleared";
        public const string ParseErrorMessage = "ResourcingGapFlagCleared audit payload could not be parsed.";
        public const string EngagementNameKey = "engagementName";
        public const string DefaultEngagement = "Engagement";
    }

    public static class ResourcingGapFlagRaised
    {
        public const string TemplateKey = "ResourcingGapFlagRaised";
        public const string ParseErrorMessage = "ResourcingGapFlagRaised audit payload could not be parsed.";
        public const string EngagementNameKey = "engagementName";
        public const string UnconfirmedCountKey = "unconfirmedCount";
        public const string DaysToBillingKey = "daysToBilling";
        public const string DefaultEngagement = "Engagement";
    }

    public static class UnauthorizedAccess
    {
        public const string TemplateKey = "UnauthorizedAccess";
        public const string ParseErrorMessage = "UnauthorizedAccess audit payload could not be parsed.";
        public const string EmailKey = "email";
        public const string Defaultunknown = "unknown";
    }

    public static class UserCreated
    {
        public const string TemplateKey = "UserCreated";
        public const string ParseErrorMessage = "UserCreated audit payload could not be parsed.";
        public const string UserNameKey = "userName";
        public const string ActorNameKey = "actorName";
    }

    public static class UserDeleted
    {
        public const string TemplateKey = "UserDeleted";
        public const string ParseErrorMessage = "UserDeleted audit payload could not be parsed.";
        public const string UserNameKey = "userName";
        public const string ActorNameKey = "actorName";
    }

    public static class UserLoggedIn
    {
        public const string TemplateKey = "UserLoggedIn";
        public const string ParseErrorMessage = "UserLoggedIn audit payload could not be parsed.";
        public const string UserNameKey = "userName";
    }

    public static class UserRoleAllocated
    {
        public const string TemplateKey = "UserRoleAllocated";
        public const string ParseErrorMessage = "UserRoleAllocated audit payload could not be parsed.";
        public const string RoleNameKey = "roleName";
        public const string TargetUserNameKey = "targetUserName";
        public const string ActorNameKey = "actorName";
        public const string DefaultRole = "Role";
        public const string DefaultUser = "User";
        public const string DefaultUnknownUser = "Unknown User";
    }

    public static class UserUpdated
    {
        public const string TemplateKey = "UserUpdated";
        public const string ParseErrorMessage = "UserUpdated audit payload could not be parsed.";
        public const string UserNameKey = "userName";
        public const string ActorNameKey = "actorName";
    }
}
