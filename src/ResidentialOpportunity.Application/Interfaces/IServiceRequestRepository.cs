using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Application.Interfaces;

public interface IServiceRequestRepository
{
    Task<ServiceRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ServiceRequest>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ServiceRequest>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(ServiceRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(ServiceRequest request, CancellationToken cancellationToken = default);
}
