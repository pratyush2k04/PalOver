using FluentValidation;
using Pdr.Api.DTOs.MasterChecklist;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Validators;

public class CreateChecklistItemRequestDtoValidator : AbstractValidator<CreateChecklistItemRequestDto>
{
    public CreateChecklistItemRequestDtoValidator(IChecklistRepository checklistRepository)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.");

        RuleFor(x => x.CategoryId)
            .MustAsync(async (categoryId, cancellationToken) =>
                await checklistRepository.GetCategoryByIdAsync(categoryId) is not null)
            .WithMessage("Selected category does not exist.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(1).WithMessage("Display order must be at least 1.");
    }
}
