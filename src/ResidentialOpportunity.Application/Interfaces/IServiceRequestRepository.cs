using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Application.Interfaces;

/// <summary>
/// Provides data access for service request entities.
/// </summary>
public interface IServiceRequestRepository
{
    /// <summary>
    /// Returns a service request by its unique identifier, or null if not found.
    /// </summary>
    Task<ServiceRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all service requests matching the given contact email, newest first.
    /// </summary>
    Task<IReadOnlyList<ServiceRequest>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages a new service request for insertion.
    /// </summary>
    Task AddAsync(ServiceRequest request, CancellationToken cancellationToken = default);
}
