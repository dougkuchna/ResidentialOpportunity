using ResidentialOpportunity.Domain.ValueObjects;

namespace ResidentialOpportunity.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public string IdentityUserId { get; private set; } = default!;
    public ContactInfo ContactInfo { get; private set; } = default!;
    public Address? Address { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    // EF Core constructor
    private Customer() { }

    public static Customer Create(string identityUserId, ContactInfo contactInfo, Address? address = null)
    {
        if (string.IsNullOrWhiteSpace(identityUserId))
            throw new ArgumentException("Identity user ID is required.", nameof(identityUserId));
        if (contactInfo is null)
            throw new ArgumentNullException(nameof(contactInfo));

        return new Customer
        {
            Id = Guid.NewGuid(),
            IdentityUserId = identityUserId,
            ContactInfo = contactInfo,
            Address = address,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
