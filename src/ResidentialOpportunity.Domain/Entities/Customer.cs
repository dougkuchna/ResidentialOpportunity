using ResidentialOpportunity.Domain.Enums;
using ResidentialOpportunity.Domain.ValueObjects;

namespace ResidentialOpportunity.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public Address Address { get; private set; } = default!;
    public string MobilePhone { get; private set; } = default!;
    public PreferredContactMethod PreferredContactMethod { get; private set; }
    public CustomerType CustomerType { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    // EF Core constructor
    private Customer() { }

    /// <summary>
    /// Creates a new Residential customer from service request data.
    /// </summary>
    public static Customer Create(
        string name,
        string email,
        Address address,
        string mobilePhone,
        PreferredContactMethod preferredContactMethod,
        CustomerType customerType = CustomerType.Residential)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        ArgumentNullException.ThrowIfNull(address);
        if (string.IsNullOrWhiteSpace(mobilePhone))
            throw new ArgumentException("Mobile phone is required.", nameof(mobilePhone));

        return new Customer
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Email = email.Trim(),
            Address = address,
            MobilePhone = mobilePhone.Trim(),
            PreferredContactMethod = preferredContactMethod,
            CustomerType = customerType,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
