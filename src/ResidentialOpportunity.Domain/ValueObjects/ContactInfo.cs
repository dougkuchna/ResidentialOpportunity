namespace ResidentialOpportunity.Domain.ValueObjects;

public class ContactInfo : IEquatable<ContactInfo>
{
    public string Name { get; }
    public string Email { get; }
    public string Phone { get; }

    public ContactInfo(string name, string email, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone is required.", nameof(phone));

        Name = name.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
    }

    // EF Core requires a parameterless constructor
    private ContactInfo() { Name = default!; Email = default!; Phone = default!; }

    public bool Equals(ContactInfo? other)
    {
        if (other is null) return false;
        return Name == other.Name && Email == other.Email && Phone == other.Phone;
    }

    public override bool Equals(object? obj) => Equals(obj as ContactInfo);

    public override int GetHashCode() => HashCode.Combine(Name, Email, Phone);
}
