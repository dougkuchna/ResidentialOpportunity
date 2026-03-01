using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Services;
using ResidentialOpportunity.Infrastructure.Data;

namespace ResidentialOpportunity.Web.Components.Pages;

public partial class MyRequests
{
    [Inject] private ServiceRequestService RequestService { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private AppDbContext DbContext { get; set; } = default!;

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
                var customer = await DbContext.Customers
                    .FirstOrDefaultAsync(c => c.IdentityUserId == userId);

                if (customer is not null)
                {
                    var results = await RequestService.GetByCustomerIdAsync(customer.Id);
                    _requests = results.ToList();
                }
            }
        }
        catch
        {
            _requests = null;
        }
        finally
        {
            _isLoading = false;
        }
    }
}
