using FluentValidation;
using SmartMeterWeb.Models;

namespace SmartMeterWeb.Validators
{
    public class CustomerCareDtoValidator : AbstractValidator<CustomerCareDto>
    {
        public CustomerCareDtoValidator()
        {
            RuleFor(x => x.ConsumerId)
                .GreaterThan(0).WithMessage("ConsumerId must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^[0-9]{10}$").WithMessage("Phone should be 10 digits");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message cannot be empty")
                .MaximumLength(250).WithMessage("Message should not exceed 250 characters");
        }

    }

}
