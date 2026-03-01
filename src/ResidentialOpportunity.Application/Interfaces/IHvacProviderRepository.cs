using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Application.Interfaces;

/// <summary>
/// Provides data access for HVAC provider entities.
/// </summary>
public interface IHvacProviderRepository
{
    /// <summary>
    /// Returns a provider with its service areas, or null if not found.
    /// </summary>
    Task<HvacProvider?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all active providers that serve the given ZIP code.
    /// </summary>
    Task<IReadOnlyList<HvacProvider>> GetByZipCodeAsync(string zipCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all active providers ordered by company name.
    /// </summary>
    Task<IReadOnlyList<HvacProvider>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages a new provider for insertion.
    /// </summary>
    Task AddAsync(HvacProvider provider, CancellationToken cancellationToken = default);
}
