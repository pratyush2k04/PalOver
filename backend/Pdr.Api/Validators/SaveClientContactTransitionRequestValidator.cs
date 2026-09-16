using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.ClientContactTransitions;

namespace Pdr.Api.Validators;

public class SaveClientContactTransitionRequestValidator : AbstractValidator<SaveClientContactTransitionRequest>
{
    public SaveClientContactTransitionRequestValidator()
    {
        RuleFor(x => x.OutgoingContactName)
            .NotEmpty().WithMessage(ClientContactTransitionValidationConstants.OutgoingContactNameRequired)
            .MaximumLength(ClientContactTransitionValidationConstants.MaxNameLength).WithMessage(ClientContactTransitionValidationConstants.NameMaxLength);

        RuleFor(x => x.OutgoingContactRole)
            .NotEmpty().WithMessage(ClientContactTransitionValidationConstants.OutgoingContactRoleRequired)
            .MaximumLength(ClientContactTransitionValidationConstants.MaxRoleLength).WithMessage(ClientContactTransitionValidationConstants.RoleMaxLength);

        RuleFor(x => x.IncomingContactName)
            .NotEmpty().WithMessage(ClientContactTransitionValidationConstants.IncomingContactNameRequired)
            .MaximumLength(ClientContactTransitionValidationConstants.MaxNameLength).WithMessage(ClientContactTransitionValidationConstants.NameMaxLength);

        RuleFor(x => x.IncomingContactRole)
            .NotEmpty().WithMessage(ClientContactTransitionValidationConstants.IncomingContactRoleRequired)
            .MaximumLength(ClientContactTransitionValidationConstants.MaxRoleLength).WithMessage(ClientContactTransitionValidationConstants.RoleMaxLength);

        RuleFor(x => x.IncomingContactEmail)
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .When(x => !string.IsNullOrEmpty(x.IncomingContactEmail));

        RuleFor(x => x.IncomingContactBackground)
            .MaximumLength(2000).WithMessage("Background must not exceed 2000 characters.");
    }
}
