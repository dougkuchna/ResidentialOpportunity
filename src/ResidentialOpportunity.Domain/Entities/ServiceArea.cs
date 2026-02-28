namespace ResidentialOpportunity.Domain.Entities;

public class ServiceArea
{
    public Guid Id { get; private set; }
    public Guid ProviderId { get; private set; }
    public string ZipCode { get; private set; } = default!;

    // Navigation property
    public HvacProvider Provider { get; private set; } = default!;

    // EF Core constructor
    private ServiceArea() { }

    public static ServiceArea Create(Guid providerId, string zipCode)
    {
        if (providerId == Guid.Empty)
            throw new ArgumentException("ProviderId is required.", nameof(providerId));
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("ZipCode is required.", nameof(zipCode));

        return new ServiceArea
        {
            Id = Guid.NewGuid(),
            ProviderId = providerId,
            ZipCode = zipCode.Trim()
        };
    }
}
