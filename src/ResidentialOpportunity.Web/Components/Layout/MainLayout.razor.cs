using MudBlazor;

namespace ResidentialOpportunity.Web.Components.Layout;

public partial class MainLayout
{
    private bool _drawerOpen = true;

    private readonly MudTheme _theme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#1a3a5c",
            Secondary = "#2980b9",
            AppbarBackground = "#1a3a5c"
        }
    };

    private void ToggleDrawer() => _drawerOpen = !_drawerOpen;
}
