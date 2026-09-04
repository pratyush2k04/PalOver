using FluentValidation;
using Pdr.Api.DTOs.EngagementChecklist;

namespace Pdr.Api.Validators;

public class SaveChecklistItemEvidenceRequestDtoValidator : AbstractValidator<SaveChecklistItemEvidenceRequestDto>
{
    public SaveChecklistItemEvidenceRequestDtoValidator()
    {
        RuleFor(x => x.DocumentId)
            .GreaterThan(0).WithMessage("A document must be selected as evidence.");
    }
}
