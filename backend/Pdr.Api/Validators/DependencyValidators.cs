using FluentValidation;
using Pdr.Api.DTOs.Dependencies;

namespace Pdr.Api.Validators;

public class CreateDependencyDtoValidator : AbstractValidator<CreateDependencyDto>
{
    public CreateDependencyDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");
            
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("A valid Type must be provided.");

        RuleFor(x => x.NeededByDate)
            .NotEmpty().WithMessage("Needed By Date is required.");
    }
}

public class UpdateDependencyDtoValidator : AbstractValidator<UpdateDependencyDto>
{
    public UpdateDependencyDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");
            
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("A valid Type must be provided.");

        RuleFor(x => x.NeededByDate)
            .NotEmpty().WithMessage("Needed By Date is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("A valid Status must be provided.");
    }
}
