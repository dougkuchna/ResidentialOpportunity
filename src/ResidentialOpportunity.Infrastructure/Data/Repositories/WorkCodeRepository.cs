using Microsoft.EntityFrameworkCore;
using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Interfaces;

namespace ResidentialOpportunity.Infrastructure.Data.Repositories;

public class WorkCodeRepository : IWorkCodeRepository
{
    private readonly LegacyDbContext _context;

    public WorkCodeRepository(LegacyDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<IReadOnlyList<WorkCodeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.WorkCodes
            .OrderBy(w => w.Description)
            .Select(w => new WorkCodeDto { Code = w.Code, Description = w.Description })
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}
