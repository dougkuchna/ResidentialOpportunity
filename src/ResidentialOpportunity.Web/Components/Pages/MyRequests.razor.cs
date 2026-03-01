using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Application.Services;

namespace ResidentialOpportunity.Web.Components.Pages;

public partial class MyRequests
{
    [Inject] private ServiceRequestService RequestService { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private ICustomerRepository CustomerRepository { get; set; } = default!;
    [Inject] private ILogger<MyRequests> Logger { get; set; } = default!;

    private List<ServiceRequestDto>? _requests;
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId is not null)
            {
                var customer = await CustomerRepository.GetByIdentityUserIdAsync(userId);

                if (customer is not null)
                {
                    var results = await RequestService.GetByCustomerIdAsync(customer.Id);
                    _requests = results.ToList();
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load service requests");
            _requests = null;
        }
        finally
        {
            _isLoading = false;
        }
    }
}
