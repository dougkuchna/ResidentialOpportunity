using Microsoft.AspNetCore.Mvc;
using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Services;

namespace ResidentialOpportunity.Web.Controllers;

[ApiController]
[Route("api/providers")]
[Produces("application/json", "application/xml")]
public class ProvidersController : ControllerBase
{
    private readonly ProviderLookupService _service;

    public ProvidersController(ProviderLookupService service)
    {
        _service = service;
    }

    /// <summary>
    /// Search for HVAC providers by ZIP code.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProviderSearchResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchByZip(
        [FromQuery] string? zipCode,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(zipCode) || zipCode.Length != 5 || !zipCode.All(char.IsDigit))
        {
            ModelState.AddModelError(nameof(zipCode), "A valid 5-digit ZIP code is required.");
            return ValidationProblem(ModelState);
        }

        var results = await _service.SearchByZipCodeAsync(zipCode, cancellationToken);
        return Ok(results);
    }
}
