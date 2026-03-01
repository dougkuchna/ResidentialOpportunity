using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Application.Mapping;

namespace ResidentialOpportunity.Application.Services;

/// <summary>
/// Searches for HVAC providers by ZIP code.
/// </summary>
public class ProviderLookupService
{
    private readonly IHvacProviderRepository _providerRepository;

    public ProviderLookupService(IHvacProviderRepository providerRepository)
    {
        ArgumentNullException.ThrowIfNull(providerRepository);
        _providerRepository = providerRepository;
    }

    /// <summary>
    /// Returns active providers that serve the given ZIP code.
    /// </summary>
    public async Task<IReadOnlyList<ProviderSearchResult>> SearchByZipCodeAsync(
        string zipCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("ZIP code is required.", nameof(zipCode));

        var providers = await _providerRepository.GetByZipCodeAsync(zipCode.Trim(), cancellationToken).ConfigureAwait(false);
        return providers.Select(p => p.ToSearchResult()).ToList();
    }
}
