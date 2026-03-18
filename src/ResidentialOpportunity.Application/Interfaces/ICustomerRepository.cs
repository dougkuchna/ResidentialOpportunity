using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Application.Interfaces;

/// <summary>
/// Provides data access for customer entities.
/// </summary>
public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
