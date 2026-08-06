using FluentValidation;
using Pdr.Api.DTOs.Kickoff;

namespace Pdr.Api.Validators;

public class KickoffActionItemDtoValidator : AbstractValidator<KickoffActionItemDto>
{
    public KickoffActionItemDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Action item title is required.")
            .MaximumLength(255).WithMessage("Title cannot exceed 255 characters.");

        RuleFor(x => x.OwnerName)
            .NotEmpty()
            .When(x => !x.AssignedToUserId.HasValue)
            .WithMessage("Owner name is required when assigned user is not specified.");

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Due date is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid action item status.");
    }
}
