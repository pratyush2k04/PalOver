namespace Pdr.Api.Constants;

public static class ArchitectureValidationConstants
{
    public const decimal SizingRiskThresholdPercentage = 20.0m;

    public static class Messages
    {
        public const string ValidationSaved = "Architecture validation details saved successfully.";
        public const string FeatureAdded = "Discovery feature item added successfully.";
        public const string FeatureUpdated = "Discovery feature item updated successfully.";
        public const string FeatureDeleted = "Discovery feature item deleted successfully.";
        public const string RationaleRequiredWhenModified = "Rationale is required when the proposed architecture is modified.";
        public const string FeatureNameRequired = "Feature name is required.";
        public const string FeatureNameMaxLength = "Feature name cannot exceed 200 characters.";
        public const string EstimateWeeksPositive = "Estimate in weeks must be greater than 0.";
        public const string EstimateWeeksMax = "Estimate in weeks cannot exceed 50 weeks.";
        public const string TargetSprintRequired = "Target sprint is required.";
        public const string TargetSprintMaxLength = "Target sprint cannot exceed 50 characters.";
        public const string InvalidArchitectureStatus = "Invalid architecture validation status value.";
    }
}
