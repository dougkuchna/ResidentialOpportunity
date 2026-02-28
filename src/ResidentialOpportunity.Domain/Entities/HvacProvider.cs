using ResidentialOpportunity.Domain.ValueObjects;

namespace ResidentialOpportunity.Domain.Entities;

public class HvacProvider
{
    public Guid Id { get; private set; }
    public string CompanyName { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? Website { get; private set; }
    public Address Address { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? LogoUrl { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<ServiceArea> _serviceAreas = [];
    public IReadOnlyCollection<ServiceArea> ServiceAreas => _serviceAreas.AsReadOnly();

    // EF Core constructor
    private HvacProvider() { }

    public static HvacProvider Create(
        string companyName,
        string phone,
        string email,
        Address address,
        string? website = null,
        string? description = null,
        string? logoUrl = null)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Company name is required.", nameof(companyName));
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone is required.", nameof(phone));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        if (address is null)
            throw new ArgumentNullException(nameof(address));

        return new HvacProvider
        {
            Id = Guid.NewGuid(),
            CompanyName = companyName.Trim(),
            Phone = phone.Trim(),
            Email = email.Trim(),
            Address = address,
            Website = website?.Trim(),
            Description = description?.Trim(),
            LogoUrl = logoUrl?.Trim(),
            IsActive = true
        };
    }

    public void AddServiceArea(string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("ZipCode is required.", nameof(zipCode));

        var trimmed = zipCode.Trim();
        if (_serviceAreas.Any(sa => sa.ZipCode == trimmed))
            return; // Already exists

        _serviceAreas.Add(ServiceArea.Create(Id, trimmed));
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
