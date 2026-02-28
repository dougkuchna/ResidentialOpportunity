using Moq;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Application.Services;
using ResidentialOpportunity.Domain.Entities;
using ResidentialOpportunity.Domain.ValueObjects;

namespace ResidentialOpportunity.Application.Tests.Services;

public class ProviderLookupServiceTests
{
    private readonly Mock<IHvacProviderRepository> _repoMock = new();
    private readonly ProviderLookupService _service;

    public ProviderLookupServiceTests()
    {
        _service = new ProviderLookupService(_repoMock.Object);
    }

    [Fact]
    public async Task SearchByZipCodeAsync_ReturnsMatchingProviders()
    {
        var provider = HvacProvider.Create(
            "Cool Air HVAC", "555-000-1111", "info@coolair.com",
            new Address("100 Industrial Blvd", "Springfield", "IL", "62704"));
        provider.AddServiceArea("62704");

        _repoMock.Setup(r => r.GetByZipCodeAsync("62704", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HvacProvider> { provider });

        var results = await _service.SearchByZipCodeAsync("62704");

        Assert.Single(results);
        Assert.Equal("Cool Air HVAC", results[0].CompanyName);
        Assert.Equal("555-000-1111", results[0].Phone);
        Assert.Equal("info@coolair.com", results[0].Email);
    }

    [Fact]
    public async Task SearchByZipCodeAsync_NoMatches_ReturnsEmptyList()
    {
        _repoMock.Setup(r => r.GetByZipCodeAsync("99999", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HvacProvider>());

        var results = await _service.SearchByZipCodeAsync("99999");

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SearchByZipCodeAsync_WithEmptyZip_ThrowsArgumentException(string? zip)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _service.SearchByZipCodeAsync(zip!));
    }

    [Fact]
    public async Task SearchByZipCodeAsync_TrimsWhitespace()
    {
        _repoMock.Setup(r => r.GetByZipCodeAsync("62704", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HvacProvider>());

        await _service.SearchByZipCodeAsync("  62704  ");

        _repoMock.Verify(r => r.GetByZipCodeAsync("62704", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchByZipCodeAsync_MapsProviderFieldsCorrectly()
    {
        var provider = HvacProvider.Create(
            "Warm Home Inc", "555-222-3333", "contact@warmhome.com",
            new Address("200 Commerce Dr", "Chicago", "IL", "60601"),
            website: "https://warmhome.com", description: "Heating specialists");

        _repoMock.Setup(r => r.GetByZipCodeAsync("60601", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HvacProvider> { provider });

        var results = await _service.SearchByZipCodeAsync("60601");

        var result = results[0];
        Assert.Equal("Warm Home Inc", result.CompanyName);
        Assert.Equal("https://warmhome.com", result.Website);
        Assert.Equal("Heating specialists", result.Description);
        Assert.Equal("Chicago", result.City);
        Assert.Equal("IL", result.State);
    }
}
