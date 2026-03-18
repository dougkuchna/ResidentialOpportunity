using Microsoft.Extensions.Logging;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Infrastructure.Data;

/// <summary>
/// Best-effort write of customer data to legacy dbo.clnt table.
/// </summary>
public class LegacyClientService : ILegacyClientService
{
    private readonly LegacyDbContext _context;
    private readonly ILogger<LegacyClientService> _logger;

    public LegacyClientService(LegacyDbContext context, ILogger<LegacyClientService> logger)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);
        _context = context;
        _logger = logger;
    }

    public async Task CreateClientAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        try
        {
            var legacyClient = new LegacyClient
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Street = customer.Address.Street,
                City = customer.Address.City,
                State = customer.Address.State,
                Zip = customer.Address.ZipCode,
                MobilePhone = customer.MobilePhone,
                PreferredContact = customer.PreferredContactMethod.ToString(),
                ClientType = customer.CustomerType.ToString(),
                CreatedAt = customer.CreatedAt
            };

            await _context.Clients.AddAsync(legacyClient, cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write client {CustomerId} to legacy dbo.clnt table", customer.Id);
        }
    }
}
