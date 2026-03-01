using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Application.Interfaces;

/// <summary>
/// Provides read access to customer data for application services.
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// Returns the customer linked to the given ASP.NET Identity user ID, or null if none exists.
    /// </summary>
    Task<Customer?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a new customer entity.
    /// </summary>
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
}
