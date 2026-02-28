using FluentValidation.TestHelper;
using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Validators;
using ResidentialOpportunity.Domain.Enums;

namespace ResidentialOpportunity.Application.Tests.Validators;

public class CreateServiceRequestValidatorTests
{
    private readonly CreateServiceRequestValidator _validator = new();

    private static CreateServiceRequestCommand ValidCommand => new()
    {
        Name = "John Doe",
        Email = "john@example.com",
        Phone = "555-123-4567",
        Street = "123 Main St",
        City = "Springfield",
        State = "IL",
        ZipCode = "62704",
        IssueDescription = "AC not cooling properly",
        IssueCategory = IssueCategory.Cooling,
        UrgencyLevel = UrgencyLevel.Standard
    };

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var result = _validator.TestValidate(ValidCommand);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- Contact Info Required ---

    [Fact]
    public void EmptyName_FailsValidation()
    {
        var cmd = ValidCommand;
        cmd.Name = "";
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void EmptyEmail_FailsValidation()
    {
        var cmd = ValidCommand;
        cmd.Email = "";
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void InvalidEmail_FailsValidation()
    {
        var cmd = ValidCommand;
        cmd.Email = "not-an-email";
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void EmptyPhone_FailsValidation()
    {
        var cmd = ValidCommand;
        cmd.Phone = "";
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    // --- Address Required ---

    [Fact]
    public void EmptyStreet_FailsValidation()
    {
        var cmd = ValidCommand;
        cmd.Street = "";
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Street);
    }

    [Fact]
    public void EmptyCity_FailsValidation()
    {
        var cmd = ValidCommand;
        cmd.City = "";
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    [Fact]
    public void StateOver2Chars_FailsValidation()
    {
        var cmd = ValidCommand;
        cmd.State = "ILL";
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.State);
    }

    [Fact]
    public void InvalidZipCode_FailsValidation()
    {
        var cmd = ValidCommand;
        cmd.ZipCode = "ABCDE";
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ZipCode);
    }

    [Theory]
    [InlineData("62704")]
    [InlineData("62704-1234")]
    public void ValidZipCode_PassesValidation(string zip)
    {
        var cmd = ValidCommand;
        cmd.ZipCode = zip;
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.ZipCode);
    }

    // --- Issue Description Required ---

    [Fact]
    public void EmptyIssueDescription_FailsValidation()
    {
        var cmd = ValidCommand;
        cmd.IssueDescription = "";
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.IssueDescription);
    }

    [Fact]
    public void IssueDescriptionOver2000Chars_FailsValidation()
    {
        var cmd = ValidCommand;
        cmd.IssueDescription = new string('x', 2001);
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.IssueDescription);
    }

    // --- Minimum Acceptable Submission ---

    [Fact]
    public void MinimumAcceptableSubmission_WithContactAndDescription_PassesValidation()
    {
        // Per requirements: minimum = contact info + description
        var cmd = new CreateServiceRequestCommand
        {
            Name = "Jane Smith",
            Email = "jane@example.com",
            Phone = "555-987-6543",
            Street = "456 Oak Ave",
            City = "Chicago",
            State = "IL",
            ZipCode = "60601",
            IssueDescription = "Heater making strange noise",
            IssueCategory = IssueCategory.Heating,
            UrgencyLevel = UrgencyLevel.Standard
        };

        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
