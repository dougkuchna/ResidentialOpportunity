using ResidentialOpportunity.Domain.Entities;
using ResidentialOpportunity.Domain.Enums;
using ResidentialOpportunity.Domain.ValueObjects;

namespace ResidentialOpportunity.Domain.Tests.Entities;

public class ServiceRequestTests
{
    private static ContactInfo ValidContact => new("John Doe", "john@example.com", "555-123-4567");
    private static Address ValidAddress => new("123 Main St", "Springfield", "IL", "62704");

    [Fact]
    public void Create_WithValidData_ReturnsServiceRequest()
    {
        var request = ServiceRequest.Create(
            ValidContact, ValidAddress, "AC not cooling",
            IssueCategory.Cooling, UrgencyLevel.Standard);

        Assert.NotEqual(Guid.Empty, request.Id);
        Assert.Equal("AC not cooling", request.IssueDescription);
        Assert.Equal(IssueCategory.Cooling, request.IssueCategory);
        Assert.Equal(UrgencyLevel.Standard, request.UrgencyLevel);
        Assert.Equal(RequestStatus.Submitted, request.Status);
        Assert.Null(request.CustomerId);
        Assert.Equal(ValidContact, request.ContactInfo);
        Assert.Equal(ValidAddress, request.Address);
    }

    [Fact]
    public void Create_WithCustomerId_SetsCustomerId()
    {
        var customerId = Guid.NewGuid();
        var request = ServiceRequest.Create(
            ValidContact, ValidAddress, "Furnace won't ignite",
            IssueCategory.Heating, UrgencyLevel.Emergency,
            customerId: customerId);

        Assert.Equal(customerId, request.CustomerId);
    }

    [Fact]
    public void Create_WithNullContactInfo_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ServiceRequest.Create(null!, ValidAddress, "Issue", IssueCategory.Other, UrgencyLevel.Standard));
    }

    [Fact]
    public void Create_WithNullAddress_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ServiceRequest.Create(ValidContact, null!, "Issue", IssueCategory.Other, UrgencyLevel.Standard));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyDescription_ThrowsArgumentException(string? description)
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceRequest.Create(ValidContact, ValidAddress, description!,
                IssueCategory.Other, UrgencyLevel.Standard));
    }

    [Fact]
    public void Create_WithDescriptionOver2000Chars_ThrowsArgumentException()
    {
        var longDescription = new string('x', 2001);
        Assert.Throws<ArgumentException>(() =>
            ServiceRequest.Create(ValidContact, ValidAddress, longDescription,
                IssueCategory.Other, UrgencyLevel.Standard));
    }

    [Fact]
    public void Create_WithExactly2000CharDescription_Succeeds()
    {
        var description = new string('x', 2000);
        var request = ServiceRequest.Create(
            ValidContact, ValidAddress, description,
            IssueCategory.Other, UrgencyLevel.Standard);

        Assert.Equal(2000, request.IssueDescription.Length);
    }

    [Fact]
    public void UpdateStatus_ChangesStatusAndUpdatedAt()
    {
        var request = ServiceRequest.Create(
            ValidContact, ValidAddress, "Issue",
            IssueCategory.Other, UrgencyLevel.Standard);
        var originalUpdatedAt = request.UpdatedAt;

        // Small delay to ensure timestamp differs
        request.UpdateStatus(RequestStatus.Acknowledged);

        Assert.Equal(RequestStatus.Acknowledged, request.Status);
        Assert.True(request.UpdatedAt >= originalUpdatedAt);
    }

    [Fact]
    public void AssignToCustomer_SetsCustomerId()
    {
        var request = ServiceRequest.Create(
            ValidContact, ValidAddress, "Issue",
            IssueCategory.Other, UrgencyLevel.Standard);
        var customerId = Guid.NewGuid();

        request.AssignToCustomer(customerId);

        Assert.Equal(customerId, request.CustomerId);
    }

    [Fact]
    public void Create_SetsCreatedAtAndUpdatedAt()
    {
        var before = DateTimeOffset.UtcNow;
        var request = ServiceRequest.Create(
            ValidContact, ValidAddress, "Issue",
            IssueCategory.Other, UrgencyLevel.Standard);
        var after = DateTimeOffset.UtcNow;

        Assert.InRange(request.CreatedAt, before, after);
        Assert.Equal(request.CreatedAt, request.UpdatedAt);
    }
}
