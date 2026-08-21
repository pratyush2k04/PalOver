using System;
using FluentValidation;
using Pdr.Api.DTOs.EngagementChecklist;

namespace Pdr.Api.Validators;

public class SaveChecklistItemEvidenceRequestDtoValidator : AbstractValidator<SaveChecklistItemEvidenceRequestDto>
{
    public SaveChecklistItemEvidenceRequestDtoValidator()
    {
        RuleFor(x => x.Note)
            .NotEmpty().WithMessage("Evidence note is required.")
            .MaximumLength(500).WithMessage("Evidence note cannot exceed 500 characters.");

        RuleFor(x => x.Link)
            .MaximumLength(2048).WithMessage("Link cannot exceed 2048 characters.")
            .Must(uri => string.IsNullOrWhiteSpace(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Link must be a valid URL.");
    }
}
