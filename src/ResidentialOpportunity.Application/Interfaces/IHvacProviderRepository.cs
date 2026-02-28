using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Application.Interfaces;

public interface IHvacProviderRepository
{
    Task<HvacProvider?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HvacProvider>> GetByZipCodeAsync(string zipCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HvacProvider>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(HvacProvider provider, CancellationToken cancellationToken = default);
}
