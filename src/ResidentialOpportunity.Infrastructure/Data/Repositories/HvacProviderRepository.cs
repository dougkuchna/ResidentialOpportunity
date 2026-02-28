using Microsoft.EntityFrameworkCore;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Infrastructure.Data.Repositories;

public class HvacProviderRepository : IHvacProviderRepository
{
    private readonly AppDbContext _context;

    public HvacProviderRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<HvacProvider?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.HvacProviders
            .Include(p => p.ServiceAreas)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<HvacProvider>> GetByZipCodeAsync(
        string zipCode, CancellationToken cancellationToken = default)
    {
        return await _context.HvacProviders
            .Include(p => p.ServiceAreas)
            .Where(p => p.IsActive && p.ServiceAreas.Any(sa => sa.ZipCode == zipCode))
            .OrderBy(p => p.CompanyName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<HvacProvider>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.HvacProviders
            .Include(p => p.ServiceAreas)
            .Where(p => p.IsActive)
            .OrderBy(p => p.CompanyName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(HvacProvider provider, CancellationToken cancellationToken = default)
    {
        await _context.HvacProviders.AddAsync(provider, cancellationToken);
    }
}
