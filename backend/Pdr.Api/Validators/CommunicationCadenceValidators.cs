using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.CommunicationCadences;

namespace Pdr.Api.Validators;

public class CreateCommunicationCadenceRequestValidator : AbstractValidator<CreateCommunicationCadenceRequest>
{
    public CreateCommunicationCadenceRequestValidator()
    {
        RuleFor(x => x.StakeholderRole)
            .NotEmpty().WithMessage(CommunicationCadenceValidationConstants.StakeholderRoleRequired)
            .MaximumLength(200).WithMessage(CommunicationCadenceValidationConstants.StakeholderRoleMaxLength);

        RuleFor(x => x.Cadence)
            .IsInEnum().WithMessage(CommunicationCadenceValidationConstants.CadenceInvalidEnum);

        RuleFor(x => x.ParticipantUserIds)
            .NotNull().WithMessage(CommunicationCadenceValidationConstants.ParticipantUserIdsRequired)
            .Must(ids => ids != null && ids.Count > 0).WithMessage(CommunicationCadenceValidationConstants.ParticipantUserIdsRequired);
    }
}
