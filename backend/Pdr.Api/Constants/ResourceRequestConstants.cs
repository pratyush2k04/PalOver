using Pdr.Api.Models.ResourceRequests;

namespace Pdr.Api.Constants;

public static class ResourceRequestConstants
{
    public const int MinJobDescriptionLength = 15;
    public const int MaxJobDescriptionLength = 500;
    public const int UrgentThresholdWorkingDays = 5;

    public static class ValidationMessages
    {
        public const string JobDescriptionRequired = "Job description is required.";
        public const string JobDescriptionLength = "Job description must be between 15 and 500 characters.";
        public const string SkillsRequired = "At least one skill must be selected.";
        public const string PeopleNeededGreaterThanZero = "Number of people must be greater than zero.";
        public const string PastRequiredByDate = "Required-by date cannot be in the past.";
        public const string AfterBillingDate = "Resources are needed on or before the billing date.";
        public const string BillingDateNotSet = "Engagement billing date must be set before raising a resource request.";
        public const string AccessDenied = "Only the Project Manager assigned to this engagement can raise resource requests.";

        public static string NotFound(int id) => $"Resource request with ID {id} was not found.";
    }

    public static class StandardRoleNames
    {
        public const string SeniorDeveloper = "Senior Developer";
        public const string Developer = "Developer";
        public const string QAEngineer = "QA Engineer";
        public const string SolutionArchitect = "Solution Architect";
        public const string BusinessAnalyst = "Business Analyst";
    }

    public static class SkillNames
    {
        public const string ReactFrontend = "React / Frontend";
        public const string AngularFrontend = "Angular / Frontend";
        public const string NodeJsBackend = "Node.js / Backend";
        public const string DotNetBackend = ".NET / Backend";
        public const string DataEngineering = "Data Engineering";
        public const string QAAutomation = "QA Automation";
        public const string DevOpsCloud = "DevOps / Cloud";
        public const string BusinessAnalysis = "Business Analysis";
    }

    public static class StatusNames
    {
        public const string Raised = "Raised";
        public const string Allocated = "Allocated";
        public const string Confirmed = "Confirmed";
    }

    public static readonly IReadOnlyDictionary<StandardRoleEnum, string> StandardRoleDisplayNames = new Dictionary<StandardRoleEnum, string>
    {
        [StandardRoleEnum.SeniorDeveloper] = StandardRoleNames.SeniorDeveloper,
        [StandardRoleEnum.Developer] = StandardRoleNames.Developer,
        [StandardRoleEnum.QAEngineer] = StandardRoleNames.QAEngineer,
        [StandardRoleEnum.SolutionArchitect] = StandardRoleNames.SolutionArchitect,
        [StandardRoleEnum.BusinessAnalyst] = StandardRoleNames.BusinessAnalyst
    };

    public static readonly IReadOnlyDictionary<SkillEnum, string> SkillDisplayNames = new Dictionary<SkillEnum, string>
    {
        [SkillEnum.ReactFrontend] = SkillNames.ReactFrontend,
        [SkillEnum.AngularFrontend] = SkillNames.AngularFrontend,
        [SkillEnum.NodeJsBackend] = SkillNames.NodeJsBackend,
        [SkillEnum.DotNetBackend] = SkillNames.DotNetBackend,
        [SkillEnum.DataEngineering] = SkillNames.DataEngineering,
        [SkillEnum.QAAutomation] = SkillNames.QAAutomation,
        [SkillEnum.DevOpsCloud] = SkillNames.DevOpsCloud,
        [SkillEnum.BusinessAnalysis] = SkillNames.BusinessAnalysis
    };

    public static readonly IReadOnlyDictionary<ResourceRequestStatus, string> StatusDisplayNames = new Dictionary<ResourceRequestStatus, string>
    {
        [ResourceRequestStatus.Raised] = StatusNames.Raised,
        [ResourceRequestStatus.Allocated] = StatusNames.Allocated,
        [ResourceRequestStatus.Confirmed] = StatusNames.Confirmed
    };
}
