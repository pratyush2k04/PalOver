using FluentValidation;
using Pdr.Api.DTOs.BrownField;

namespace Pdr.Api.Validators;

public class CreateBrownfieldFindingRequestValidator : AbstractValidator<CreateBrownfieldFindingRequest>
{
    public CreateBrownfieldFindingRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.Severity)
            .IsInEnum().WithMessage("Severity must be a valid value.");
    }
}
