using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.ArchitectureValidation;

namespace Pdr.Api.Validators;

public class UpdateDiscoveryFeatureItemRequestValidator : AbstractValidator<UpdateDiscoveryFeatureItemRequest>
{
    public UpdateDiscoveryFeatureItemRequestValidator()
    {
        RuleFor(x => x.FeatureName)
            .NotEmpty().WithMessage(ArchitectureValidationConstants.Messages.FeatureNameRequired)
            .MaximumLength(200).WithMessage(ArchitectureValidationConstants.Messages.FeatureNameMaxLength);

        RuleFor(x => x.EstimateWeeks)
            .GreaterThan(0).WithMessage(ArchitectureValidationConstants.Messages.EstimateWeeksPositive)
            .LessThanOrEqualTo(50).WithMessage(ArchitectureValidationConstants.Messages.EstimateWeeksMax);

        RuleFor(x => x.TargetSprint)
            .NotEmpty().WithMessage(ArchitectureValidationConstants.Messages.TargetSprintRequired)
            .MaximumLength(50).WithMessage(ArchitectureValidationConstants.Messages.TargetSprintMaxLength);
    }
}
