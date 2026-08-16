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
    }
}
