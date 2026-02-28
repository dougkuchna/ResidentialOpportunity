using ResidentialOpportunity.Domain.Enums;
using ResidentialOpportunity.Domain.ValueObjects;

namespace ResidentialOpportunity.Domain.Entities;

public class ServiceRequest
{
    public Guid Id { get; private set; }
    public ContactInfo ContactInfo { get; private set; } = default!;
    public Address Address { get; private set; } = default!;
    public string IssueDescription { get; private set; } = default!;
    public IssueCategory IssueCategory { get; private set; }
    public UrgencyLevel UrgencyLevel { get; private set; }
    public string? EquipmentDetails { get; private set; }
    public string? PreferredSchedule { get; private set; }
    public RequestStatus Status { get; private set; }
    public Guid? CustomerId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    // EF Core constructor
    private ServiceRequest() { }

    public static ServiceRequest Create(
        ContactInfo contactInfo,
        Address address,
        string issueDescription,
        IssueCategory issueCategory,
        UrgencyLevel urgencyLevel,
        string? equipmentDetails = null,
        string? preferredSchedule = null,
        Guid? customerId = null)
    {
        if (contactInfo is null)
            throw new ArgumentNullException(nameof(contactInfo));
        if (address is null)
            throw new ArgumentNullException(nameof(address));
        if (string.IsNullOrWhiteSpace(issueDescription))
            throw new ArgumentException("Issue description is required.", nameof(issueDescription));
        if (issueDescription.Length > 2000)
            throw new ArgumentException("Issue description must not exceed 2000 characters.", nameof(issueDescription));

        var now = DateTimeOffset.UtcNow;
        return new ServiceRequest
        {
            Id = Guid.NewGuid(),
            ContactInfo = contactInfo,
            Address = address,
            IssueDescription = issueDescription.Trim(),
            IssueCategory = issueCategory,
            UrgencyLevel = urgencyLevel,
            EquipmentDetails = equipmentDetails?.Trim(),
            PreferredSchedule = preferredSchedule?.Trim(),
            Status = RequestStatus.Submitted,
            CustomerId = customerId,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void UpdateStatus(RequestStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AssignToCustomer(Guid customerId)
    {
        CustomerId = customerId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
