using FluentValidation;
using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Application.Mapping;

namespace ResidentialOpportunity.Application.Services;

/// <summary>
/// Orchestrates creation, retrieval, and customer-claiming of service requests.
/// </summary>
public class ServiceRequestService
{
    private readonly IServiceRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateServiceRequestCommand> _validator;

    public ServiceRequestService(
        IServiceRequestRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateServiceRequestCommand> validator)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(validator);
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    /// <summary>
    /// Validates and persists a new service request, returning the created DTO.
    /// </summary>
    public async Task<ServiceRequestDto> CreateAsync(
        CreateServiceRequestCommand command,
        Guid? customerId = null,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken).ConfigureAwait(false);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var entity = command.ToDomainEntity(customerId);
        await _repository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return entity.ToDto();
    }

    /// <summary>
    /// Returns a service request by its unique identifier, or null if not found.
    /// </summary>
    public async Task<ServiceRequestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return entity?.ToDto();
    }

    /// <summary>
    /// Returns all service requests owned by the specified customer.
    /// </summary>
    public async Task<IReadOnlyList<ServiceRequestDto>> GetByCustomerIdAsync(
        Guid customerId, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetByCustomerIdAsync(customerId, cancellationToken).ConfigureAwait(false);
        return entities.Select(e => e.ToDto()).ToList();
    }

    /// <summary>
    /// Returns all service requests matching the given email address.
    /// </summary>
    public async Task<IReadOnlyList<ServiceRequestDto>> GetByEmailAsync(
        string email, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetByEmailAsync(email, cancellationToken).ConfigureAwait(false);
        return entities.Select(e => e.ToDto()).ToList();
    }

    /// <summary>
    /// Assigns all unclaimed requests matching the email to the given customer.
    /// Returns the number of requests claimed.
    /// </summary>
    public async Task<int> ClaimRequestsForCustomerAsync(
        Guid customerId, string email, CancellationToken cancellationToken = default)
    {
        var unclaimed = await _repository.GetByEmailAsync(email, cancellationToken).ConfigureAwait(false);
        var toClaim = unclaimed.Where(r => r.CustomerId is null).ToList();

        foreach (var request in toClaim)
        {
            request.AssignToCustomer(customerId);
            await _repository.UpdateAsync(request, cancellationToken).ConfigureAwait(false);
        }

        if (toClaim.Count > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return toClaim.Count;
    }
}
