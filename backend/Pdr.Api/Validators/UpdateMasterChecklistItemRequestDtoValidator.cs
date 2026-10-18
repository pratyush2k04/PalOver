using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.MasterChecklist;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Validators;

public class UpdateMasterChecklistItemRequestDtoValidator : AbstractValidator<UpdateChecklistItemRequestDto>
{
    public UpdateMasterChecklistItemRequestDtoValidator(IMasterChecklistRepository checklistRepository)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(MasterChecklistConstants.Messages.TitleRequired);

        RuleFor(x => x.CategoryId)
            .MustAsync(async (categoryId, cancellationToken) =>
                await checklistRepository.GetCategoryByIdAsync(categoryId) is not null)
            .WithMessage(MasterChecklistConstants.Messages.CategoryNotFound);
    }
}
