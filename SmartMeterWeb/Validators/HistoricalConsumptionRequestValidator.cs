using FluentValidation;
using SmartMeterWeb.Models.Reports;

namespace SmartMeterWeb.Validators
{
    public class HistoricalConsumptionRequestValidator : AbstractValidator<HistoricalConsumptionRequestDto>
    {
        public HistoricalConsumptionRequestValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow.Date)
                    .WithMessage("Date cannot be in the future.")
                .GreaterThan(new DateTime(2020, 1, 1))
                    .WithMessage("Date cannot be before 2020.");

            RuleFor(x => x.OrgUnitId)
                .GreaterThan(0)
                .When(x => x.OrgUnitId.HasValue)
                .WithMessage("OrgUnitId must be greater than 0 if provided.");
        }
    }
}
