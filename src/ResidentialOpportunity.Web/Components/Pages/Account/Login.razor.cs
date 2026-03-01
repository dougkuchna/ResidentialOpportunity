using Microsoft.AspNetCore.Components;

namespace ResidentialOpportunity.Web.Components.Pages.Account;

public partial class Login
{
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private string? _errorMessage;

    protected override void OnInitialized()
    {
        var uri = new Uri(Navigation.Uri);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        _errorMessage = query["error"];
    }
}
