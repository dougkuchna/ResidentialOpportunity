namespace ResidentialOpportunity.Application.Interfaces;

/// <summary>
/// Validates US ZIP codes against a known dataset.
/// ZIP code data sourced from SimpleMaps (https://simplemaps.com/data/us-zips).
/// </summary>
public interface IZipCodeValidationService
{
    Task<ZipCodeValidationResult> ValidateAsync(string zipCode, CancellationToken cancellationToken = default);
}

public record ZipCodeValidationResult(
    bool IsValid,
    string? City,
    string? StateId,
    string? StateName,
    string? ErrorMessage);
