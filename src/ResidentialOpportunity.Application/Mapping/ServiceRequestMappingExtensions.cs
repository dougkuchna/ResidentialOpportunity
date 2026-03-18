using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Domain.Entities;
using ResidentialOpportunity.Domain.ValueObjects;

namespace ResidentialOpportunity.Application.Mapping;

/// <summary>
/// Maps between <see cref="ServiceRequest"/> entities and application DTOs.
/// </summary>
public static class ServiceRequestMappingExtensions
{
    /// <summary>
    /// Converts a create command into a new domain entity.
    /// </summary>
    public static ServiceRequest ToDomainEntity(this CreateServiceRequestCommand command, Guid? customerId = null)
    {
        var contactInfo = new ContactInfo(command.Name, command.Email, command.Phone);
        var address = new Address(command.Street, command.City, command.State, command.ZipCode);

        return ServiceRequest.Create(
            contactInfo,
            address,
            command.IssueDescription,
            command.IssueCategory,
            command.UrgencyLevel,
            command.EquipmentDetails,
            command.PreferredSchedule,
            command.WorkCodeCode,
            customerId);
    }

    /// <summary>
    /// Projects a service request entity into a read-only DTO.
    /// </summary>
    public static ServiceRequestDto ToDto(this ServiceRequest entity)
    {
        return new ServiceRequestDto
        {
            Id = entity.Id,
            Name = entity.ContactInfo.Name,
            Email = entity.ContactInfo.Email,
            Phone = entity.ContactInfo.Phone,
            Street = entity.Address.Street,
            City = entity.Address.City,
            State = entity.Address.State,
            ZipCode = entity.Address.ZipCode,
            IssueDescription = entity.IssueDescription,
            IssueCategory = entity.IssueCategory,
            UrgencyLevel = entity.UrgencyLevel,
            WorkCodeCode = entity.WorkCodeCode,
            EquipmentDetails = entity.EquipmentDetails,
            PreferredSchedule = entity.PreferredSchedule,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt
        };
    }
}
