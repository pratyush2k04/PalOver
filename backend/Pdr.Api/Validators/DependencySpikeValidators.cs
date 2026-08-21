using FluentValidation;
using Pdr.Api.DTOs.DependencySpikes;

namespace Pdr.Api.Validators;

public class CreateDependencySpikeRequestValidator : AbstractValidator<CreateDependencySpikeRequest>
{
    public CreateDependencySpikeRequestValidator()
    {
        RuleFor(x => x.DependencyName)
            .NotEmpty().WithMessage("Dependency name is required.")
            .MaximumLength(200).WithMessage("Dependency name must not exceed 200 characters.");

        RuleFor(x => x.Objective)
            .NotEmpty().WithMessage("Objective is required.")
            .MaximumLength(1000).WithMessage("Objective must not exceed 1000 characters.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid DependencySpikeStatus.");

        RuleFor(x => x.OutcomeFindings)
            .MaximumLength(2000).WithMessage("Outcome findings must not exceed 2000 characters.");
    }
}

public class UpdateDependencySpikeRequestValidator : AbstractValidator<UpdateDependencySpikeRequest>
{
    public UpdateDependencySpikeRequestValidator()
    {
        RuleFor(x => x.DependencyName)
            .NotEmpty().WithMessage("Dependency name is required.")
            .MaximumLength(200).WithMessage("Dependency name must not exceed 200 characters.");

        RuleFor(x => x.Objective)
            .NotEmpty().WithMessage("Objective is required.")
            .MaximumLength(1000).WithMessage("Objective must not exceed 1000 characters.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid DependencySpikeStatus.");

        RuleFor(x => x.OutcomeFindings)
            .MaximumLength(2000).WithMessage("Outcome findings must not exceed 2000 characters.");
    }
}
