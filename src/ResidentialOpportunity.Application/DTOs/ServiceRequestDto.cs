using System.Xml.Serialization;
using ResidentialOpportunity.Domain.Enums;

namespace ResidentialOpportunity.Application.DTOs;

[XmlRoot("ServiceRequest")]
public class ServiceRequestDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Street { get; set; } = default!;
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public string ZipCode { get; set; } = default!;
    public string IssueDescription { get; set; } = default!;
    public IssueCategory IssueCategory { get; set; }
    public UrgencyLevel UrgencyLevel { get; set; }
    public string? EquipmentDetails { get; set; }
    public string? PreferredSchedule { get; set; }
    public RequestStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
