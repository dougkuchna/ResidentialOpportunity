using ResidentialOpportunity.Domain.ValueObjects;

namespace ResidentialOpportunity.Domain.Tests.ValueObjects;

public class AddressTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var address = new Address("123 Main St", "Springfield", "IL", "62704");

        Assert.Equal("123 Main St", address.Street);
        Assert.Equal("Springfield", address.City);
        Assert.Equal("IL", address.State);
        Assert.Equal("62704", address.ZipCode);
    }

    [Fact]
    public void Create_TrimsWhitespace()
    {
        var address = new Address("  123 Main St  ", "  Springfield  ", "  IL  ", "  62704  ");

        Assert.Equal("123 Main St", address.Street);
        Assert.Equal("Springfield", address.City);
        Assert.Equal("IL", address.State);
        Assert.Equal("62704", address.ZipCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyStreet_ThrowsArgumentException(string? street)
    {
        Assert.Throws<ArgumentException>(() => new Address(street!, "Springfield", "IL", "62704"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyZipCode_ThrowsArgumentException(string? zipCode)
    {
        Assert.Throws<ArgumentException>(() => new Address("123 Main St", "Springfield", "IL", zipCode!));
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        var a = new Address("123 Main St", "Springfield", "IL", "62704");
        var b = new Address("123 Main St", "Springfield", "IL", "62704");

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        var a = new Address("123 Main St", "Springfield", "IL", "62704");
        var b = new Address("456 Oak Ave", "Chicago", "IL", "60601");

        Assert.NotEqual(a, b);
    }
}
