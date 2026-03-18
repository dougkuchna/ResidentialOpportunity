using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using ResidentialOpportunity.Web.Configuration;

namespace ResidentialOpportunity.Web.Components.Layout;

public partial class MainLayout
{
    [Inject] private IOptions<BrandingOptions> BrandingOptionsAccessor { get; set; } = default!;

    private MudTheme _theme = default!;

    protected override void OnInitialized()
    {
        var branding = BrandingOptionsAccessor.Value;
        _theme = new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = branding.PrimaryColor,
                Secondary = branding.SecondaryColor
            }
        };
    }
}
