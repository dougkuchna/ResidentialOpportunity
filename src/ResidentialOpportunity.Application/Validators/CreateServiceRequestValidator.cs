using FluentValidation;
using ResidentialOpportunity.Application.DTOs;

namespace ResidentialOpportunity.Application.Validators;

public class CreateServiceRequestValidator : AbstractValidator<CreateServiceRequestCommand>
{
    public CreateServiceRequestValidator()
    {
        // Contact Info — required
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(254);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(20);

        // Address — required
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street address is required.")
            .MaximumLength(300);

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100);

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.")
            .MaximumLength(2).WithMessage("Use 2-letter state abbreviation.");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("ZIP code is required.")
            .Matches(@"^\d{5}(-\d{4})?$").WithMessage("ZIP code must be in 5-digit or ZIP+4 format.");

        // Issue Description — required
        RuleFor(x => x.IssueDescription)
            .NotEmpty().WithMessage("Issue description is required.")
            .MaximumLength(2000).WithMessage("Issue description must not exceed 2000 characters.");

        // Enums — valid range
        RuleFor(x => x.IssueCategory).IsInEnum();
        RuleFor(x => x.UrgencyLevel).IsInEnum();

        // Optional fields
        RuleFor(x => x.EquipmentDetails).MaximumLength(500);
        RuleFor(x => x.PreferredSchedule).MaximumLength(200);
    }
}
