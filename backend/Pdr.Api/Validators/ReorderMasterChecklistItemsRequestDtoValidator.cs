using FluentValidation;
using Pdr.Api.DTOs.MasterChecklist;

namespace Pdr.Api.Validators;

public class ReorderMasterChecklistItemsRequestDtoValidator : AbstractValidator<ReorderMasterChecklistItemsRequestDto>
{
    public ReorderMasterChecklistItemsRequestDtoValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required to reorder.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.DisplayOrder)
                .GreaterThanOrEqualTo(1).WithMessage("Display order must be at least 1.");
        });
    }
}
