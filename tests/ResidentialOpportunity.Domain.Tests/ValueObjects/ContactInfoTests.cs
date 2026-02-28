using ResidentialOpportunity.Domain.ValueObjects;

namespace ResidentialOpportunity.Domain.Tests.ValueObjects;

public class ContactInfoTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var contact = new ContactInfo("John Doe", "john@example.com", "555-123-4567");

        Assert.Equal("John Doe", contact.Name);
        Assert.Equal("john@example.com", contact.Email);
        Assert.Equal("555-123-4567", contact.Phone);
    }

    [Fact]
    public void Create_TrimsWhitespace()
    {
        var contact = new ContactInfo("  John Doe  ", "  john@example.com  ", "  555-123-4567  ");

        Assert.Equal("John Doe", contact.Name);
        Assert.Equal("john@example.com", contact.Email);
        Assert.Equal("555-123-4567", contact.Phone);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ThrowsArgumentException(string? name)
    {
        Assert.Throws<ArgumentException>(() => new ContactInfo(name!, "john@example.com", "555-123-4567"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyEmail_ThrowsArgumentException(string? email)
    {
        Assert.Throws<ArgumentException>(() => new ContactInfo("John Doe", email!, "555-123-4567"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyPhone_ThrowsArgumentException(string? phone)
    {
        Assert.Throws<ArgumentException>(() => new ContactInfo("John Doe", "john@example.com", phone!));
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        var a = new ContactInfo("John Doe", "john@example.com", "555-123-4567");
        var b = new ContactInfo("John Doe", "john@example.com", "555-123-4567");

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        var a = new ContactInfo("John Doe", "john@example.com", "555-123-4567");
        var b = new ContactInfo("Jane Doe", "jane@example.com", "555-999-0000");

        Assert.NotEqual(a, b);
    }
}
