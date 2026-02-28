# ResidentialOpportunity — HVAC Service Request Portal

A customer-facing portal for residential HVAC service requests that connects homeowners with local HVAC providers. Built with **Blazor Server** on **.NET 10** using **Clean Architecture**.

## Features

- **Service Request Submission** — Full form with contact info, address, issue category/urgency, and description. Supports JSON and XML file upload to pre-fill fields.
- **Provider Lookup** — ZIP code-based search to find local HVAC service providers with contact details.
- **Request Confirmation** — Post-submission view with request summary and auto-matched providers in the customer's area.
- **Dual Format API** — Accepts and returns both JSON and XML via content negotiation (API controllers planned for Phase 5).
- **Swagger/OpenAPI** — Interactive API documentation at `/swagger`.
- **Comprehensive Test Suite** — 86 unit and integration tests covering domain, application, and infrastructure layers.

## Architecture

The project follows **Clean Architecture** with four layers:

```
ResidentialOpportunity/
├── src/
│   ├── ResidentialOpportunity.Domain/          # Entities, Value Objects, Enums
│   ├── ResidentialOpportunity.Application/     # DTOs, Services, Interfaces, Validators
│   ├── ResidentialOpportunity.Infrastructure/  # EF Core, Repositories, Seed Data
│   └── ResidentialOpportunity.Web/             # Blazor Server UI, Program.cs
├── tests/
│   ├── ResidentialOpportunity.Domain.Tests/
│   ├── ResidentialOpportunity.Application.Tests/
│   └── ResidentialOpportunity.Infrastructure.Tests/
└── ResidentialOpportunity.slnx
```

### Domain Layer
- **Entities**: `ServiceRequest`, `Customer`, `HvacProvider`, `ServiceArea`
- **Value Objects**: `ContactInfo`, `Address`
- **Enums**: `IssueCategory` (Heating, Cooling, Ventilation, Thermostat, AirQuality, Maintenance, Emergency, Other), `UrgencyLevel` (Low, Standard, Urgent, Emergency), `RequestStatus`

### Application Layer
- **Services**: `ServiceRequestService`, `ProviderLookupService`
- **DTOs**: `CreateServiceRequestCommand`, `ServiceRequestDto`, `ProviderSearchResult`
- **Validation**: FluentValidation with `CreateServiceRequestValidator` enforcing required contact info and issue description
- **Interfaces**: `IServiceRequestRepository`, `IHvacProviderRepository`, `IUnitOfWork`

### Infrastructure Layer
- **EF Core**: `AppDbContext` (extends `IdentityDbContext`) with fluent API configurations, owned entity types for value objects, enum-to-string conversions
- **Identity**: ASP.NET Core Identity with `IdentityUser`/`IdentityRole` stored in the same database
- **Repositories**: `ServiceRequestRepository`, `HvacProviderRepository`
- **Seed Data**: 5 sample HVAC providers across Illinois and Texas for development

### Web Layer (Blazor Server + API)
- **Pages**: Home, SubmitRequest (with EditForm + file upload), FindProviders (ZIP search), RequestConfirmation, MyRequests (authenticated)
- **Auth Pages**: Register, Login, Logout (static SSR for cookie auth compatibility)
- **API Controllers**: `ServiceRequestsController`, `ProvidersController` with JSON/XML content negotiation
- **Layout**: Auth-aware sidebar navigation with `AuthorizeView`
- **Program.cs**: DI registration, Identity, XML/JSON formatters, Swagger, database seeding

## Tech Stack

| Component | Technology |
|---|---|
| Framework | .NET 10 / ASP.NET Core 10 |
| UI | Blazor Server (Interactive Server rendering) |
| ORM | Entity Framework Core 10 |
| Database | SQL Server (LocalDB for development) |
| Validation | FluentValidation 12.1.1 |
| API Docs | Swashbuckle.AspNetCore 10.1.4 |
| Auth | ASP.NET Core Identity (cookie-based) |
| Testing | xUnit 3.1.4, Moq 4.20.72, EF Core InMemory |
| Solution Format | .slnx (XML-based, .NET 10+) |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (10.0.100 or later)
- SQL Server LocalDB (included with Visual Studio) or SQL Server instance

## Getting Started

### Clone and Build

```bash
git clone https://github.com/<your-username>/ResidentialOpportunity.git
cd ResidentialOpportunity
dotnet build
```

### Run Tests

```bash
dotnet test
```

### Run the Application

```bash
dotnet run --project src/ResidentialOpportunity.Web
```

The app will be available at `https://localhost:5001` (or the port shown in console output).

### Database

The application uses SQL Server LocalDB by default. The connection string is in `src/ResidentialOpportunity.Web/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ResidentialOpportunity;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

On first run, `SeedData.InitializeAsync()` populates the database with sample HVAC providers using `EnsureCreated()`.

## Project Status

### Completed Phases

- **Phase 1**: Project scaffolding — Solution structure, 4 source projects, 3 test projects, project references
- **Phase 2**: Application layer — Interfaces, DTOs, services, FluentValidation validator, mapping extensions
- **Phase 3**: Infrastructure layer — EF Core DbContext, entity configurations, repositories, seed data
- **Phase 4**: Web layer (Blazor UI) — Home, SubmitRequest, FindProviders, RequestConfirmation pages with styling
- **Phase 5**: API Controllers — `ServiceRequestsController` (POST/GET JSON/XML), `ProvidersController` (GET by ZIP), content negotiation, RFC 7807 error responses
- **Phase 6**: Authentication — ASP.NET Core Identity, Register/Login/Logout pages, MyRequests page, anonymous-to-authenticated request claiming, auth-aware NavMenu and SubmitRequest
- **Phase 8**: Testing — 86 tests across domain, application, and infrastructure layers

### Remaining Phases

- **Phase 7**: Docker & Deployment — Multi-stage Dockerfile, docker-compose with SQL Server, Azure deployment config

## API Endpoints

All API endpoints support both JSON and XML via `Accept` and `Content-Type` headers. Swagger UI is available at `/swagger` in development.

### Service Requests

- **`POST /api/service-requests`** — Create a new service request
  - Body: `CreateServiceRequestCommand` (JSON or XML)
  - Returns: `201 Created` with `ServiceRequestDto` and `Location` header
  - Errors: `400` with RFC 7807 validation problem details

- **`GET /api/service-requests/{id}`** — Get a service request by ID
  - Returns: `200` with `ServiceRequestDto` or `404`

- **`GET /api/service-requests?email={email}`** — Lookup requests by email
  - Returns: `200` with list of `ServiceRequestDto`

### Providers

- **`GET /api/providers?zipCode={zip}`** — Search providers by ZIP code
  - Returns: `200` with list of `ProviderSearchResult` or `400` if invalid ZIP

### Example: Create a request via curl

```bash
curl -X POST http://localhost:5239/api/service-requests \
  -H "Content-Type: application/json" \
  -d '{
    "Name": "Jane Doe",
    "Email": "jane@example.com",
    "Phone": "555-987-6543",
    "Street": "456 Oak Ave",
    "City": "Springfield",
    "State": "IL",
    "ZipCode": "62704",
    "IssueCategory": "Cooling",
    "UrgencyLevel": "Urgent",
    "IssueDescription": "AC not cooling"
  }'
```

### Example: Search providers

```bash
curl http://localhost:5239/api/providers?zipCode=62704
```

## Sample Data (Seed Providers)

The seed data includes 5 HVAC providers for development:

1. **Springfield Heating & Cooling** — Springfield, IL (62701-62707)
2. **Central Illinois HVAC** — Decatur, IL (62521-62526)
3. **Dallas Comfort Systems** — Dallas, TX (75201-75212)
4. **Lone Star Air Services** — Austin, TX (73301, 78701-78705)
5. **Houston Climate Control** — Houston, TX (77001-77010)

## File Upload Format

The Submit Request page accepts JSON or XML files to pre-fill the form. Example:

### JSON
```json
{
  "Name": "Jane Doe",
  "Email": "jane@example.com",
  "Phone": "555-987-6543",
  "Street": "456 Oak Ave",
  "City": "Springfield",
  "State": "IL",
  "ZipCode": "62704",
  "IssueCategory": "Cooling",
  "UrgencyLevel": "Urgent",
  "IssueDescription": "AC unit making loud grinding noise and not cooling"
}
```

### XML
```xml
<?xml version="1.0" encoding="utf-8"?>
<CreateServiceRequestCommand>
  <Name>Jane Doe</Name>
  <Email>jane@example.com</Email>
  <Phone>555-987-6543</Phone>
  <Street>456 Oak Ave</Street>
  <City>Springfield</City>
  <State>IL</State>
  <ZipCode>62704</ZipCode>
  <IssueCategory>Cooling</IssueCategory>
  <UrgencyLevel>Urgent</UrgencyLevel>
  <IssueDescription>AC unit making loud grinding noise and not cooling</IssueDescription>
</CreateServiceRequestCommand>
```

## License

This project is proprietary. All rights reserved.
