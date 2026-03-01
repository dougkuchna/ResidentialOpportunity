using System.Globalization;
using ResidentialOpportunity.Application.Interfaces;

namespace ResidentialOpportunity.Infrastructure.Data;

/// <summary>
/// Validates ZIP codes using the SimpleMaps US ZIP code database (https://simplemaps.com/data/us-zips).
/// Data is loaded once from an embedded CSV resource and cached in memory for fast lookups.
/// </summary>
public sealed class CsvZipCodeValidationService : IZipCodeValidationService
{
    private readonly Lazy<Dictionary<string, ZipEntry>> _zipLookup;

    public CsvZipCodeValidationService()
    {
        _zipLookup = new Lazy<Dictionary<string, ZipEntry>>(LoadZipCodes);
    }

    public Task<ZipCodeValidationResult> ValidateAsync(
        string zipCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return Task.FromResult(new ZipCodeValidationResult(
                false, null, null, null, "ZIP code is required."));
        }

        var trimmed = zipCode.Trim();

        if (trimmed.Length != 5 || !trimmed.All(char.IsDigit))
        {
            return Task.FromResult(new ZipCodeValidationResult(
                false, null, null, null, "ZIP code must be exactly 5 digits."));
        }

        if (_zipLookup.Value.TryGetValue(trimmed, out var entry))
        {
            return Task.FromResult(new ZipCodeValidationResult(
                true, entry.City, entry.StateId, entry.StateName, null));
        }

        return Task.FromResult(new ZipCodeValidationResult(
            false, null, null, null, $"ZIP code {trimmed} was not found. Please verify and try again."));
    }

    private static Dictionary<string, ZipEntry> LoadZipCodes()
    {
        var assembly = typeof(CsvZipCodeValidationService).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("uszips.csv", StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException(
                "Embedded resource 'uszips.csv' not found. Ensure it is included as an EmbeddedResource.");

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);

        var lookup = new Dictionary<string, ZipEntry>(StringComparer.Ordinal);

        // Skip header line
        var header = reader.ReadLine();
        if (header is null) return lookup;

        // Parse header to find column indices (defensive against column reordering)
        var columns = ParseCsvLine(header);
        var zipIdx = Array.IndexOf(columns, "zip");
        var cityIdx = Array.IndexOf(columns, "city");
        var stateIdIdx = Array.IndexOf(columns, "state_id");
        var stateNameIdx = Array.IndexOf(columns, "state_name");

        if (zipIdx < 0 || cityIdx < 0 || stateIdIdx < 0 || stateNameIdx < 0)
            throw new InvalidOperationException(
                "uszips.csv is missing required columns: zip, city, state_id, state_name.");

        while (reader.ReadLine() is { } line)
        {
            var fields = ParseCsvLine(line);
            if (fields.Length <= Math.Max(Math.Max(zipIdx, cityIdx), Math.Max(stateIdIdx, stateNameIdx)))
                continue;

            var zip = fields[zipIdx];
            if (zip.Length == 5 && !lookup.ContainsKey(zip))
            {
                lookup[zip] = new ZipEntry(fields[cityIdx], fields[stateIdIdx], fields[stateNameIdx]);
            }
        }

        return lookup;
    }

    /// <summary>
    /// Simple CSV field parser that handles quoted fields (needed because uszips.csv quotes all values).
    /// </summary>
    private static string[] ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var i = 0;

        while (i < line.Length)
        {
            if (line[i] == '"')
            {
                // Quoted field
                i++; // skip opening quote
                var start = i;
                var value = new System.Text.StringBuilder();
                while (i < line.Length)
                {
                    if (line[i] == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            // Escaped quote
                            value.Append(line.AsSpan(start, i - start));
                            value.Append('"');
                            i += 2;
                            start = i;
                        }
                        else
                        {
                            // End of quoted field
                            value.Append(line.AsSpan(start, i - start));
                            i++; // skip closing quote
                            break;
                        }
                    }
                    else
                    {
                        i++;
                    }
                }
                fields.Add(value.ToString());
                // Skip comma after quoted field
                if (i < line.Length && line[i] == ',') i++;
            }
            else
            {
                // Unquoted field
                var start = i;
                while (i < line.Length && line[i] != ',') i++;
                fields.Add(line[start..i]);
                if (i < line.Length) i++; // skip comma
            }
        }

        return fields.ToArray();
    }

    private sealed record ZipEntry(string City, string StateId, string StateName);
}
