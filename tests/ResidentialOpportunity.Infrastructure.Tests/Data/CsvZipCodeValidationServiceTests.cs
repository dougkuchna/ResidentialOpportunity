using ResidentialOpportunity.Infrastructure.Data;

namespace ResidentialOpportunity.Infrastructure.Tests.Data;

public class CsvZipCodeValidationServiceTests
{
    private readonly CsvZipCodeValidationService _service = new();

    [Fact]
    public async Task ValidateAsync_KnownZipCode_ReturnsValid()
    {
        // 62704 = Springfield, IL (in seed data)
        var result = await _service.ValidateAsync("62704");

        Assert.True(result.IsValid);
        Assert.Equal("Springfield", result.City);
        Assert.Equal("IL", result.StateId);
        Assert.Equal("Illinois", result.StateName);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task ValidateAsync_AnotherKnownZip_ReturnsValid()
    {
        // 10001 = New York, NY
        var result = await _service.ValidateAsync("10001");

        Assert.True(result.IsValid);
        Assert.NotNull(result.City);
        Assert.Equal("NY", result.StateId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task ValidateAsync_UnknownZipCode_ReturnsInvalid()
    {
        var result = await _service.ValidateAsync("99999");

        Assert.False(result.IsValid);
        Assert.Null(result.City);
        Assert.Null(result.StateId);
        Assert.Contains("99999", result.ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ValidateAsync_NullOrWhitespace_ReturnsInvalid(string? zipCode)
    {
        var result = await _service.ValidateAsync(zipCode!);

        Assert.False(result.IsValid);
        Assert.Equal("ZIP code is required.", result.ErrorMessage);
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("123456")]
    [InlineData("abcde")]
    [InlineData("1234a")]
    public async Task ValidateAsync_InvalidFormat_ReturnsFormatError(string zipCode)
    {
        var result = await _service.ValidateAsync(zipCode);

        Assert.False(result.IsValid);
        Assert.Equal("ZIP code must be exactly 5 digits.", result.ErrorMessage);
    }

    [Fact]
    public async Task ValidateAsync_WithWhitespace_TrimsAndValidates()
    {
        var result = await _service.ValidateAsync("  62704  ");

        Assert.True(result.IsValid);
        Assert.Equal("Springfield", result.City);
    }

    [Fact]
    public async Task ValidateAsync_PuertoRicoZip_ReturnsValid()
    {
        // 00601 = Adjuntas, PR (first entry in CSV)
        var result = await _service.ValidateAsync("00601");

        Assert.True(result.IsValid);
        Assert.Equal("Adjuntas", result.City);
        Assert.Equal("PR", result.StateId);
        Assert.Equal("Puerto Rico", result.StateName);
    }

    [Fact]
    public async Task ValidateAsync_IsThreadSafe()
    {
        // Run many concurrent validations to confirm thread-safe Lazy initialization
        var tasks = Enumerable.Range(0, 50)
            .Select(_ => _service.ValidateAsync("62704"))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        Assert.All(results, r =>
        {
            Assert.True(r.IsValid);
            Assert.Equal("Springfield", r.City);
        });
    }
}
