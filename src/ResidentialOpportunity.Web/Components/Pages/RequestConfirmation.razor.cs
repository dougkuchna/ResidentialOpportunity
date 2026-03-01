using Microsoft.AspNetCore.Components;
using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Services;

namespace ResidentialOpportunity.Web.Components.Pages;

public partial class RequestConfirmation
{
    [Inject] private ServiceRequestService RequestService { get; set; } = default!;
    [Inject] private ProviderLookupService ProviderService { get; set; } = default!;

    [Parameter] public Guid RequestId { get; set; }

    private ServiceRequestDto? _request;
    private List<ProviderSearchResult>? _providers;
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _request = await RequestService.GetByIdAsync(RequestId);

            if (_request is not null && !string.IsNullOrEmpty(_request.ZipCode))
            {
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
