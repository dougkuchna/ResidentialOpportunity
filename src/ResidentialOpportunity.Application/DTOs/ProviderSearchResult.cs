using System.Xml.Serialization;

namespace ResidentialOpportunity.Application.DTOs;

[XmlRoot("Provider")]
public class ProviderSearchResult
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Website { get; set; }
    public string? Description { get; set; }
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
}
