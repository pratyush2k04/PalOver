using FluentValidation;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.MasterChecklist;

namespace Pdr.Api.Validators;

public class ReorderMasterChecklistItemsRequestDtoValidator : AbstractValidator<ReorderMasterChecklistItemsRequestDto>
{
    public ReorderMasterChecklistItemsRequestDtoValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage(MasterChecklistConstants.Messages.ItemsRequired);

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.DisplayOrder)
                .GreaterThanOrEqualTo(1).WithMessage(MasterChecklistConstants.Messages.DisplayOrderMinimum);
        });
    }
}
