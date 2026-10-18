using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Auth;

namespace Pdr.Api.Validators;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(AuthValidationConstants.EmailRequired)
            .EmailAddress().WithMessage(AuthValidationConstants.InvalidWorkEmail);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(AuthValidationConstants.PasswordRequired);
    }
}
