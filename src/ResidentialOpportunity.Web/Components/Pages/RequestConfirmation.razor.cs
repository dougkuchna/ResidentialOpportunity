using Microsoft.AspNetCore.Components;
using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Application.Services;

namespace ResidentialOpportunity.Web.Components.Pages;

public partial class RequestConfirmation
{
    [Inject] private ServiceRequestService RequestService { get; set; } = default!;
    [Inject] private ProviderLookupService ProviderService { get; set; } = default!;
    [Inject] private IZipCodeValidationService ZipValidator { get; set; } = default!;

    [Parameter] public Guid RequestId { get; set; }

    private ServiceRequestDto? _request;
    private List<ProviderSearchResult>? _providers;
    private string? _validatedLocation;
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _request = await RequestService.GetByIdAsync(RequestId);

            if (_request is not null && !string.IsNullOrEmpty(_request.ZipCode))
            {
                var validation = await ZipValidator.ValidateAsync(_request.ZipCode);
                if (validation.IsValid)
                {
                    _validatedLocation = $"{validation.City}, {validation.StateId}";
                }

                var results = await ProviderService.SearchByZipCodeAsync(_request.ZipCode);
                _providers = results.ToList();
            }
        }
        catch
        {
            _request = null;
        }
        finally
        {
            _isLoading = false;
        }
    }
}
