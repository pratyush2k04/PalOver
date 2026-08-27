using FluentValidation;
using Pdr.Api.DTOs.Kpi;

namespace Pdr.Api.Validators;

public class KpiReportRequestDtoValidator : AbstractValidator<KpiReportRequestDto>
{
    public KpiReportRequestDtoValidator()
    {
        RuleFor(x => x.DatePreset)
            .IsInEnum().WithMessage("Unrecognized date preset.");

        When(x => x.DatePreset == KpiReportDatePreset.Custom, () =>
        {
            RuleFor(x => x.CustomFrom)
                .NotNull().WithMessage("CustomFrom is required when DatePreset is Custom.");

            RuleFor(x => x.CustomTo)
                .NotNull().WithMessage("CustomTo is required when DatePreset is Custom.");

            RuleFor(x => x)
                .Must(x => x.CustomFrom!.Value <= x.CustomTo!.Value)
                .WithMessage("CustomFrom must be on or before CustomTo.")
                .When(x => x.CustomFrom.HasValue && x.CustomTo.HasValue);
        });

        RuleForEach(x => x.PmUserIds)
            .GreaterThan(0).WithMessage("PM user id must be a positive integer.");

        RuleForEach(x => x.EngagementTypeIds)
            .GreaterThan(0).WithMessage("Engagement type id must be a positive integer.");
    }
}
