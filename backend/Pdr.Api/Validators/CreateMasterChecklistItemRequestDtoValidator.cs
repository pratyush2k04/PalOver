using FluentValidation;
using Pdr.Api.DTOs.MasterChecklist;
using Pdr.Api.Repositories.Interfaces;

using Pdr.Api.Constants;

namespace Pdr.Api.Validators;

public class CreateMasterChecklistItemRequestDtoValidator : AbstractValidator<MasterCreateChecklistItemRequestDto>
{
    public CreateMasterChecklistItemRequestDtoValidator(IMasterChecklistRepository checklistRepository)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(MasterChecklistConstants.Messages.TitleRequired);

        RuleFor(x => x.CategoryId)
            .MustAsync(async (categoryId, cancellationToken) =>
                await checklistRepository.GetCategoryByIdAsync(categoryId) is not null)
            .WithMessage(MasterChecklistConstants.Messages.CategoryNotFound);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(1).WithMessage(MasterChecklistConstants.Messages.DisplayOrderMinimum);
    }
}
