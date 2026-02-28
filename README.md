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
- **EF Core**: `AppDbContext` with fluent API configurations, owned entity types for value objects, enum-to-string conversions
- **Repositories**: `ServiceRequestRepository`, `HvacProviderRepository`
- **Seed Data**: 5 sample HVAC providers across Illinois and Texas for development

### Web Layer (Blazor Server)
- **Pages**: Home, SubmitRequest (with EditForm + file upload), FindProviders (ZIP search), RequestConfirmation
- **Layout**: Sidebar navigation with responsive main content area
- **Program.cs**: DI registration, XML/JSON formatters, Swagger, database seeding

## Tech Stack

| Component | Technology |
|---|---|
| Framework | .NET 10 / ASP.NET Core 10 |
| UI | Blazor Server (Interactive Server rendering) |
| ORM | Entity Framework Core 10 |
| Database | SQL Server (LocalDB for development) |
| Validation | FluentValidation 12.1.1 |
| API Docs | Swashbuckle.AspNetCore 10.1.4 |
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
- **Phase 8**: Testing — 86 tests across domain, application, and infrastructure layers

### Remaining Phases

- **Phase 5**: API Controllers — `ServiceRequestsController` (POST JSON/XML), `ProvidersController` (GET by ZIP), content negotiation
- **Phase 6**: Authentication — ASP.NET Core Identity, Register/Login pages, MyRequests page, anonymous-to-authenticated claiming
- **Phase 7**: Docker & Deployment — Multi-stage Dockerfile, docker-compose with SQL Server, Azure deployment config

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
