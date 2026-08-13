using FluentValidation;
using Pdr.Api.DTOs.Kickoff;

namespace Pdr.Api.Validators;

public class SaveKickoffRecordRequestValidator : AbstractValidator<SaveKickoffRecordRequest>
{
    public SaveKickoffRecordRequestValidator()
    {
        RuleFor(x => x.Outcomes)
            .NotEmpty().WithMessage("Kickoff outcomes summary is required.");
    }
}
