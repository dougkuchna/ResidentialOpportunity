# ZIP Code Validation

## Problem

Service request forms accept a ZIP code as part of the customer's address. The system needs to validate that the ZIP code is a real US ZIP code before accepting the submission, providing clear feedback when an invalid ZIP is entered.

## Current Solution: CSV-Based Validation (SimpleMaps)

ZIP codes are validated against the [SimpleMaps US ZIP Codes Database](https://simplemaps.com/data/us-zips), which contains 33,782 US ZIP codes with city, state, county, lat/lng, and other metadata. The CSV is embedded in the Infrastructure assembly and loaded into a `Dictionary<string, ZipEntry>` on first use.

### How It Works

1. The service request form can validate ZIP codes against the CSV dataset
2. If valid: the city/state can be displayed for confirmation
3. If invalid: an error message is shown ("ZIP code 99999 was not found")

### Architecture

- **Interface**: `IZipCodeValidationService` in `Application/Interfaces/`
- **Implementation**: `CsvZipCodeValidationService` in `Infrastructure/Data/`
- **Data**: `uszips.csv` embedded as `EmbeddedResource` in Infrastructure assembly
- **DI**: Registered as singleton in `DependencyInjection.AddInfrastructure()`
- **Tests**: 8 tests in `Infrastructure.Tests/Data/CsvZipCodeValidationServiceTests.cs`

### Data Attribution

ZIP code data sourced from [SimpleMaps US ZIP Codes Database](https://simplemaps.com/data/us-zips).

## Future: USPS Addresses API v3

For production-grade validation with real-time USPS data, the `IZipCodeValidationService` interface can be swapped to a `UspsZipCodeValidationService` that calls the USPS Addresses API v3 City/State Lookup endpoint. The interface is already designed to support this swap.

### USPS API Endpoints

| Purpose | Method | URL |
|---|---|---|
| OAuth Token | POST | `https://apis.usps.com/oauth2/v3/token` |
| City/State Lookup | GET | `https://apis.usps.com/addresses/v3/city-state?ZIPCode={zip}` |
| Address Validation | GET | `https://apis.usps.com/addresses/v3/address?streetAddress=...&city=...&state=...&ZIPCode=...` |

### Prerequisites for USPS API

1. Create a free USPS.com account at https://www.usps.com
2. Access the Business Customer Gateway at https://gateway.usps.com
3. Register at the USPS Developer Portal at https://developers.usps.com
4. Register your application to get Consumer Key and Consumer Secret
5. Obtain a Customer Registration ID (CRID) and Mailer ID (MID)

Note: The old USPS Web Tools APIs (XML-based) were retired January 25, 2026. Use the new REST APIs only.

## References

- SimpleMaps US ZIP Codes: https://simplemaps.com/data/us-zips
- USPS Developer Portal: https://developers.usps.com
- USPS API Catalog: https://developers.usps.com/apis
- USPS API Examples (GitHub): https://github.com/USPS/api-examples
