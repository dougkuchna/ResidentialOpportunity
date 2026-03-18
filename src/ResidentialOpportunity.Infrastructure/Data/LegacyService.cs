using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Infrastructure.Data;

/// <summary>
/// Writes submission data to legacy tables. Failures propagate to block the submission.
/// </summary>
public class LegacyService : ILegacyService
{
    private readonly LegacyDbContext _context;

    public LegacyService(LegacyDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task CreateLegacyRecordsAsync(
        Customer customer,
        ServiceRequest serviceRequest,
        CancellationToken cancellationToken = default)
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

        var clientSite = new LegacyClientSite
        {
            Id = Guid.NewGuid(),
            ClientId = customer.Id,
            Street = serviceRequest.Address.Street,
            City = serviceRequest.Address.City,
            State = serviceRequest.Address.State,
            Zip = serviceRequest.Address.ZipCode,
            CreatedAt = serviceRequest.CreatedAt
        };

        var webLog = new LegacyWebLog
        {
            Id = serviceRequest.Id,
            ClientId = customer.Id,
            ClientSiteId = clientSite.Id,
            WorkCodeCode = serviceRequest.WorkCodeCode,
            Name = serviceRequest.ContactInfo.Name,
            Email = serviceRequest.ContactInfo.Email,
            Phone = serviceRequest.ContactInfo.Phone,
            Description = serviceRequest.IssueDescription,
            IssueCategory = serviceRequest.IssueCategory.ToString(),
            Urgency = serviceRequest.UrgencyLevel.ToString(),
            EquipmentDetails = serviceRequest.EquipmentDetails,
            PreferredSchedule = serviceRequest.PreferredSchedule,
            Status = serviceRequest.Status.ToString(),
            SubmittedAt = serviceRequest.CreatedAt
        };

        await _context.Clients.AddAsync(legacyClient, cancellationToken).ConfigureAwait(false);
        await _context.ClientSites.AddAsync(clientSite, cancellationToken).ConfigureAwait(false);
        await _context.WebLogs.AddAsync(webLog, cancellationToken).ConfigureAwait(false);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
