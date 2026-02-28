using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Application.Mapping;

namespace ResidentialOpportunity.Application.Services;

public class ProviderLookupService
{
    private readonly IHvacProviderRepository _providerRepository;

    public ProviderLookupService(IHvacProviderRepository providerRepository)
    {
        _providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
    }

    public async Task<IReadOnlyList<ProviderSearchResult>> SearchByZipCodeAsync(
        string zipCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("ZIP code is required.", nameof(zipCode));

        var providers = await _providerRepository.GetByZipCodeAsync(zipCode.Trim(), cancellationToken);
        return providers.Select(p => p.ToSearchResult()).ToList();
    }
}
