using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Application.Mapping;

/// <summary>
/// Maps <see cref="HvacProvider"/> entities to application DTOs.
/// </summary>
public static class ProviderMappingExtensions
{
    /// <summary>
    /// Projects a provider entity into a search result DTO.
    /// </summary>
    public static ProviderSearchResult ToSearchResult(this HvacProvider provider)
    {
        return new ProviderSearchResult
        {
            Id = provider.Id,
            CompanyName = provider.CompanyName,
            Phone = provider.Phone,
            Email = provider.Email,
            Website = provider.Website,
            Description = provider.Description,
            City = provider.Address.City,
            State = provider.Address.State
        };
    }
}
