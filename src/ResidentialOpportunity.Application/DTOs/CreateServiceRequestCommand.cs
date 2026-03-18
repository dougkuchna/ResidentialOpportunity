using System.Xml.Serialization;
using ResidentialOpportunity.Domain.Enums;

namespace ResidentialOpportunity.Application.DTOs;

/// <summary>
/// Command to create a new HVAC service request. Supports JSON and XML deserialization.
/// </summary>
[XmlRoot("ServiceRequest")]
public record class CreateServiceRequestCommand
{
    // Contact Info
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string? MobilePhone { get; set; }
    public PreferredContactMethod PreferredContactMethod { get; set; }

    // Address
    public string Street { get; set; } = default!;
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public string ZipCode { get; set; } = default!;

    // Issue Details
    public string IssueDescription { get; set; } = default!;
    public IssueCategory IssueCategory { get; set; }
    public UrgencyLevel UrgencyLevel { get; set; }
    public string? WorkCodeCode { get; set; }
    public string? EquipmentDetails { get; set; }
    public string? PreferredSchedule { get; set; }
}
