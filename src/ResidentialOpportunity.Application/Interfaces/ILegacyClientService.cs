using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Application.Interfaces;

/// <summary>
/// Best-effort write of customer data to the legacy dbo.clnt table.
/// </summary>
public interface ILegacyClientService
{
    /// <summary>
    /// Writes a client record to the legacy database. Failures are logged but do not block submission.
    /// </summary>
    Task CreateClientAsync(Customer customer, CancellationToken cancellationToken = default);
}
