using ResidentialOpportunity.Application.DTOs;

namespace ResidentialOpportunity.Application.Interfaces;

/// <summary>
/// Reads work codes from the legacy database.
/// </summary>
public interface IWorkCodeRepository
{
    /// <summary>
    /// Returns all available work codes.
    /// </summary>
    Task<IReadOnlyList<WorkCodeDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
