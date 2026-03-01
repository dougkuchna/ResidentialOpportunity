# USPS ZIP Code Validation — Integration Plan

## Problem

`ProviderLookupService.SearchByZipCodeAsync` queries the database for HVAC providers whose `ServiceArea` includes the given ZIP code. When a valid ZIP code has no providers, the result is an empty list — indistinguishable from an invalid ZIP code. Users need clear feedback: "no providers in your area" vs "that's not a real ZIP code."

## Solution: USPS Addresses API v3 — City/State Lookup

The USPS Addresses API v3 provides a **City/State Lookup** endpoint that returns the city and state for a given ZIP code. If USPS returns data, the ZIP is valid. If it returns an error, the ZIP is invalid.

- **Old Web Tools APIs** (XML-based at `secure.shippingapis.com`) — **retired January 25, 2026**. Do not use.
- **New REST APIs** (at `apis.usps.com`) — current, OAuth 2.0-protected, JSON responses.

### API Endpoints

| Purpose | Method | URL |
|---|---|---|
| OAuth Token | POST | `https://apis.usps.com/oauth2/v3/token` |
| City/State Lookup | GET | `https://apis.usps.com/addresses/v3/city-state?ZIPCode={zip}` |
| Address Validation | GET | `https://apis.usps.com/addresses/v3/address?streetAddress=...&city=...&state=...&ZIPCode=...` |
| ZIP Code Lookup | GET | `https://apis.usps.com/addresses/v3/zipcode?...` |

### Authentication (OAuth 2.0 Client Credentials)

All USPS APIs require a Bearer token obtained via OAuth 2.0 client credentials flow:

```
POST https://apis.usps.com/oauth2/v3/token
Content-Type: application/json

{
  "client_id": "<CONSUMER_KEY>",
  "client_secret": "<CONSUMER_SECRET>",
  "grant_type": "client_credentials"
}
```

Response includes `access_token` and `expires_in`. Tokens expire and must be refreshed periodically.

### Test Environment

Replace `apis.usps.com` with `apis-tem.usps.com` for testing.

## Prerequisites — USPS Account Setup

1. **Create a free USPS.com account** at https://www.usps.com
2. **Access the Business Customer Gateway** at https://gateway.usps.com — create a USPS.com business account
3. **Register at the USPS Developer Portal** at https://developers.usps.com — click Sign Up / Login
4. **Register your application** — retrieve your **Consumer Key** and **Consumer Secret**
5. **Obtain a Customer Registration ID (CRID) and Mailer ID (MID)** — contact your USPS Business Customer Gateway BSA if needed

## Proposed Integration Design

### New Interface: `IZipCodeValidationService`

Location: `src/ResidentialOpportunity.Application/Interfaces/`

```csharp
public interface IZipCodeValidationService
{
    Task<ZipCodeValidationResult> ValidateZipCodeAsync(string zipCode, CancellationToken cancellationToken = default);
}

public record ZipCodeValidationResult(bool IsValid, string? City, string? State, string? ErrorMessage);
```

### New Implementation: `UspsZipCodeValidationService`

Location: `src/ResidentialOpportunity.Infrastructure/ExternalServices/`

- Uses `HttpClient` (via `IHttpClientFactory`) to call USPS APIs
- Handles OAuth token acquisition and caching
- Calls City/State Lookup endpoint
- Returns city/state on success, or error on invalid ZIP

### Configuration

Add to `appsettings.json`:

```json
"Usps": {
  "BaseUrl": "https://apis.usps.com",
  "ConsumerKey": "<from-user-secrets>",
  "ConsumerSecret": "<from-user-secrets>"
}
```

Store secrets via `dotnet user-secrets`:

```powershell
dotnet user-secrets set "Usps:ConsumerKey" "<your-key>" --project src/ResidentialOpportunity.Web
dotnet user-secrets set "Usps:ConsumerSecret" "<your-secret>" --project src/ResidentialOpportunity.Web
```

### UI Changes

Update `FindProviders.razor.cs` and `RequestConfirmation.razor.cs` to:

1. Validate the ZIP code against USPS before/alongside provider search
2. Display city/state from USPS to confirm the location ("Searching providers in **Springfield, IL**...")
3. Show clear error if ZIP is invalid ("ZIP code 99999 is not recognized by USPS")
4. Show "no providers in your area" only for valid ZIPs with no coverage

### DI Registration

In `Program.cs`:

```csharp
builder.Services.AddHttpClient<UspsZipCodeValidationService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Usps:BaseUrl"]!);
});
builder.Services.AddScoped<IZipCodeValidationService, UspsZipCodeValidationService>();
```

## References

- USPS Developer Portal: https://developers.usps.com
- USPS API Catalog: https://developers.usps.com/apis
- USPS API Examples (GitHub): https://github.com/USPS/api-examples
- Addresses API v3 Docs: https://developers.usps.com/addressesv3

## Status

- [ ] USPS account and API credentials obtained
- [ ] `IZipCodeValidationService` interface created
- [ ] `UspsZipCodeValidationService` implemented
- [ ] OAuth token caching implemented
- [ ] Configuration and user-secrets wired up
- [ ] UI updated to use validation
- [ ] Unit tests written
- [ ] Integration tested against USPS test environment
