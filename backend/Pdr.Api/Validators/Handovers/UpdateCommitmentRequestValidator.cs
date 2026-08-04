using FluentValidation;
using Pdr.Api.DTOs.Handovers;

namespace Pdr.Api.Validators.Handovers;

public class UpdateCommitmentRequestValidator : AbstractValidator<UpdateCommitmentRequest>
{
    private static readonly string[] ValidTimelineUnits = { "Days", "Weeks", "Months" };

    public UpdateCommitmentRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.CommitmentTypeId)
            .GreaterThan(0).WithMessage("Commitment Type ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(x => x.Timeline)
            .GreaterThan(0).WithMessage("Timeline must be greater than 0.");

        RuleFor(x => x.TimelineUnit)
            .Must(unit => ValidTimelineUnits.Contains(unit))
            .WithMessage("Timeline Unit must be one of: Days, Weeks, Months.");
    }
}
