using FluentValidation;
using Pdr.Api.DTOs.Profile;

namespace Pdr.Api.Validators;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(10).WithMessage("Password must be at least 10 characters long.")
            .Must(p => p.Any(char.IsUpper)).WithMessage("Password must contain an uppercase letter.")
            .Must(p => p.Any(char.IsLower)).WithMessage("Password must contain a lowercase letter.")
            .Must(p => p.Any(char.IsDigit)).WithMessage("Password must contain a number.")
            .Must(p => p.Any(c => !char.IsLetterOrDigit(c))).WithMessage("Password must contain a symbol.");
    }
}
