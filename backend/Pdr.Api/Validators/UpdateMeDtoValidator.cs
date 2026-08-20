using FluentValidation;
using Pdr.Api.DTOs.Me;

namespace Pdr.Api.Validators;

public class UpdateMeDtoValidator : AbstractValidator<UpdateMeDto>
{
    public UpdateMeDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(200).WithMessage("Full name must not exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Enter a valid work email address.")
            .EmailAddress().WithMessage("Enter a valid work email address.");
    }
}
