using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Documents;

namespace Pdr.Api.Validators;

public class RemoveDocumentRequestDtoValidator : AbstractValidator<RemoveDocumentRequestDto>
{
    public RemoveDocumentRequestDtoValidator()
    {
        RuleFor(x => x.Reason)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(DocumentValidationConstants.Messages.RemovalReasonRequired)
            .MinimumLength(DocumentValidationConstants.MinRemovalReasonLength).WithMessage(DocumentValidationConstants.Messages.RemovalReasonMinLength)
            .MaximumLength(DocumentValidationConstants.MaxRemovalReasonLength).WithMessage(DocumentValidationConstants.Messages.RemovalReasonMaxLength);
    }
}
