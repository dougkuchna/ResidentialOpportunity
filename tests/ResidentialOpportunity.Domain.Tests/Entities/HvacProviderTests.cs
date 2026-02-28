using ResidentialOpportunity.Domain.Entities;
using ResidentialOpportunity.Domain.ValueObjects;

namespace ResidentialOpportunity.Domain.Tests.Entities;

public class HvacProviderTests
{
    private static Address ValidAddress => new("100 Industrial Blvd", "Springfield", "IL", "62704");

    [Fact]
    public void Create_WithValidData_ReturnsProvider()
    {
        var provider = HvacProvider.Create(
            "Cool Air HVAC", "555-000-1111", "info@coolair.com", ValidAddress,
            website: "https://coolair.com", description: "Full-service HVAC");

        Assert.NotEqual(Guid.Empty, provider.Id);
        Assert.Equal("Cool Air HVAC", provider.CompanyName);
        Assert.Equal("555-000-1111", provider.Phone);
        Assert.Equal("info@coolair.com", provider.Email);
        Assert.Equal("https://coolair.com", provider.Website);
        Assert.True(provider.IsActive);
        Assert.Empty(provider.ServiceAreas);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCompanyName_ThrowsArgumentException(string? name)
    {
        Assert.Throws<ArgumentException>(() =>
            HvacProvider.Create(name!, "555-000-1111", "info@test.com", ValidAddress));
    }

    [Fact]
    public void Create_WithNullAddress_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            HvacProvider.Create("Test HVAC", "555-000-1111", "info@test.com", null!));
    }

    [Fact]
    public void AddServiceArea_AddsZipCode()
    {
        var provider = HvacProvider.Create("Test HVAC", "555-000-1111", "info@test.com", ValidAddress);

        provider.AddServiceArea("62704");

        Assert.Single(provider.ServiceAreas);
        Assert.Equal("62704", provider.ServiceAreas.First().ZipCode);
    }

    [Fact]
    public void AddServiceArea_DuplicateZipCode_DoesNotAddTwice()
    {
        var provider = HvacProvider.Create("Test HVAC", "555-000-1111", "info@test.com", ValidAddress);

        provider.AddServiceArea("62704");
        provider.AddServiceArea("62704");

        Assert.Single(provider.ServiceAreas);
    }

    [Fact]
    public void AddServiceArea_MultipleZipCodes_AddsAll()
    {
        var provider = HvacProvider.Create("Test HVAC", "555-000-1111", "info@test.com", ValidAddress);

        provider.AddServiceArea("62704");
        provider.AddServiceArea("62705");
        provider.AddServiceArea("62706");

        Assert.Equal(3, provider.ServiceAreas.Count);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddServiceArea_WithEmptyZip_ThrowsArgumentException(string? zip)
    {
        var provider = HvacProvider.Create("Test HVAC", "555-000-1111", "info@test.com", ValidAddress);
        Assert.Throws<ArgumentException>(() => provider.AddServiceArea(zip!));
    }

    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var provider = HvacProvider.Create("Test HVAC", "555-000-1111", "info@test.com", ValidAddress);

        provider.Deactivate();

        Assert.False(provider.IsActive);
    }

    [Fact]
    public void Activate_AfterDeactivate_SetsIsActiveTrue()
    {
        var provider = HvacProvider.Create("Test HVAC", "555-000-1111", "info@test.com", ValidAddress);
        provider.Deactivate();

        provider.Activate();

        Assert.True(provider.IsActive);
    }
}
