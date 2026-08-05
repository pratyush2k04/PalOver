using FluentValidation;
using Pdr.Api.DTOs.People;

namespace Pdr.Api.Validators;

public class PersonDtoValidator : AbstractValidator<PersonDto>
{
    public PersonDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .MaximumLength(100).WithMessage("Role must not exceed 100 characters.");

        RuleFor(x => x.PersonType)
            .IsInEnum().WithMessage("PersonType must be a valid type (Internal or Client).");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email format is invalid.")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
