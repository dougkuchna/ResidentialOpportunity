using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ResidentialOpportunity.Application.DTOs;
using ResidentialOpportunity.Application.Services;

namespace ResidentialOpportunity.Web.Controllers;

[ApiController]
[Route("api/service-requests")]
[Produces("application/json", "application/xml")]
public class ServiceRequestsController : ControllerBase
{
    private readonly ServiceRequestService _service;

    public ServiceRequestsController(ServiceRequestService service)
    {
        _service = service;
    }

    /// <summary>
    /// Create a new HVAC service request.
    /// </summary>
    [HttpPost]
    [Consumes("application/json", "application/xml")]
    [ProducesResponseType(typeof(ServiceRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateServiceRequestCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.CreateAsync(command, cancellationToken: cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return ValidationProblem(ModelState);
        }
    }

    /// <summary>
    /// Get a service request by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ServiceRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Look up service requests by email address.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ServiceRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByEmail(
        [FromQuery] string? email,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError(nameof(email), "Email query parameter is required.");
            return ValidationProblem(ModelState);
        }

        var results = await _service.GetByEmailAsync(email, cancellationToken);
        return Ok(results);
    }
}
