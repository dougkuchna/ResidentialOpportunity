namespace ResidentialOpportunity.Application.Interfaces;

/// <summary>
/// Commits pending changes as a single transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persists all staged changes and returns the number of affected rows.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
