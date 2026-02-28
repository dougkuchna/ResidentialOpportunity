using FluentValidation;
using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Application.Mapping;

namespace ResidentialOpportunity.Application.Services;

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
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ServiceRequestDto> CreateAsync(
        CreateServiceRequestCommand command,
        Guid? customerId = null,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var entity = command.ToDomainEntity(customerId);
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }

    public async Task<ServiceRequestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity?.ToDto();
    }

    public async Task<IReadOnlyList<ServiceRequestDto>> GetByCustomerIdAsync(
        Guid customerId, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetByCustomerIdAsync(customerId, cancellationToken);
        return entities.Select(e => e.ToDto()).ToList();
    }

    public async Task<IReadOnlyList<ServiceRequestDto>> GetByEmailAsync(
        string email, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetByEmailAsync(email, cancellationToken);
        return entities.Select(e => e.ToDto()).ToList();
    }
}
