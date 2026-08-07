using FluentValidation;
using Pdr.Api.DTOs.Documents;

namespace Pdr.Api.Validators;

public class RemoveDocumentRequestDtoValidator : AbstractValidator<RemoveDocumentRequestDto>
{
    public RemoveDocumentRequestDtoValidator()
    {
        RuleFor(x => x.Reason)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Reason for document removal is required.")
            .MinimumLength(10).WithMessage("Reason must be at least 10 characters.")
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
    }
}
