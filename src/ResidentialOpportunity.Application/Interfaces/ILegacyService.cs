using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Application.Interfaces;

/// <summary>
/// Writes submission data to legacy tables (dbo.clnt, dbo.clntste, dbo.wblg).
/// Failures propagate and block the submission.
/// </summary>
public interface ILegacyService
{
    /// <summary>
    /// Creates legacy client, client site, and web log records for a submission.
    /// </summary>
    Task CreateLegacyRecordsAsync(Customer customer, ServiceRequest serviceRequest, CancellationToken cancellationToken = default);
}
