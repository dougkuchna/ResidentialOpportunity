using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Services;

namespace ResidentialOpportunity.Web.Components.Pages;

public partial class FindProviders
{
    [Inject] private ProviderLookupService ProviderService { get; set; } = default!;

    private string _zipCode = string.Empty;
    private string _lastSearchedZip = string.Empty;
    private bool _isSearching;
    private bool _hasSearched;
    private string? _errorMessage;
    private List<ProviderSearchResult>? _providers;

    private async Task SearchProviders()
    {
        _errorMessage = null;

        if (string.IsNullOrWhiteSpace(_zipCode) || _zipCode.Length != 5 || !_zipCode.All(char.IsDigit))
        {
            _errorMessage = "Please enter a valid 5-digit ZIP code.";
            return;
        }

        _isSearching = true;
        _lastSearchedZip = _zipCode;

        try
        {
            var results = await ProviderService.SearchByZipCodeAsync(_zipCode);
            _providers = results.ToList();
            _hasSearched = true;
        }
        catch (Exception ex)
        {
            _errorMessage = $"Error searching providers: {ex.Message}";
        }
        finally
        {
            _isSearching = false;
        }
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SearchProviders();
        }
    }
}
