namespace ResidentialOpportunity.Web.Configuration;

/// <summary>
/// Configurable branding for the embedded service request form.
/// Bind from the "Branding" section of appsettings.json.
/// </summary>
public class BrandingOptions
{
    public const string SectionName = "Branding";

    /// <summary>
    /// The HVAC client's company name displayed in the form header.
    /// </summary>
    public string CompanyName { get; set; } = "HVAC Service";

    /// <summary>
    /// Primary theme color (hex, e.g. "#1a3a5c").
    /// </summary>
    public string PrimaryColor { get; set; } = "#1a3a5c";

    /// <summary>
    /// Secondary/accent theme color (hex, e.g. "#2980b9").
    /// </summary>
    public string SecondaryColor { get; set; } = "#2980b9";

    /// <summary>
    /// Optional URL to the client's logo image.
    /// </summary>
    public string? LogoUrl { get; set; }
}
