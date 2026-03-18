using FluentValidation;
using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Application.Mapping;

namespace ResidentialOpportunity.Application.Services;

/// <summary>
/// Orchestrates creation of service requests and associated customers.
/// </summary>
public class ServiceRequestService
{
    private readonly IServiceRequestRepository _repository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateServiceRequestCommand> _validator;
    private readonly ILegacyService _legacyService;

    public ServiceRequestService(
        IServiceRequestRepository repository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateServiceRequestCommand> validator,
        ILegacyService legacyService)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(customerRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(legacyService);
        _repository = repository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _legacyService = legacyService;
    }

    /// <summary>
    /// Validates and persists a new service request + customer, returning the created DTO.
    /// </summary>
    public async Task<ServiceRequestDto> CreateAsync(
        CreateServiceRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken).ConfigureAwait(false);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // Create domain objects in memory
        var address = new Domain.ValueObjects.Address(command.Street, command.City, command.State, command.ZipCode);
        var customer = Domain.Entities.Customer.Create(
            command.Name,
            command.Email,
            address,
            command.MobilePhone ?? command.Phone,
            command.PreferredContactMethod);
        var entity = command.ToDomainEntity(customer.Id);

        // Write legacy records first — failures block the submission
        await _legacyService.CreateLegacyRecordsAsync(customer, entity, cancellationToken).ConfigureAwait(false);

        // Persist to local database
        await _customerRepository.AddAsync(customer, cancellationToken).ConfigureAwait(false);
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
    /// Returns all service requests matching the given email address.
    /// </summary>
    public async Task<IReadOnlyList<ServiceRequestDto>> GetByEmailAsync(
        string email, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetByEmailAsync(email, cancellationToken).ConfigureAwait(false);
        return entities.Select(e => e.ToDto()).ToList();
    }
}
