using Microsoft.EntityFrameworkCore;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Infrastructure.Data.Repositories;

public class ServiceRequestRepository : IServiceRequestRepository
{
    private readonly AppDbContext _context;

    public ServiceRequestRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ServiceRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ServiceRequests.FirstOrDefaultAsync(sr => sr.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ServiceRequest>> GetByCustomerIdAsync(
        Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.ServiceRequests
            .Where(sr => sr.CustomerId == customerId)
            .OrderByDescending(sr => sr.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ServiceRequest>> GetByEmailAsync(
        string email, CancellationToken cancellationToken = default)
    {
        return await _context.ServiceRequests
            .Where(sr => sr.ContactInfo.Email == email)
            .OrderByDescending(sr => sr.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ServiceRequest request, CancellationToken cancellationToken = default)
    {
        await _context.ServiceRequests.AddAsync(request, cancellationToken);
    }

    public Task UpdateAsync(ServiceRequest request, CancellationToken cancellationToken = default)
    {
        _context.ServiceRequests.Update(request);
        return Task.CompletedTask;
    }
}
