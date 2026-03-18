# ResidentialOpportunity — Embeddable HVAC Service Request Form

A standalone residential HVAC service request form designed to be embedded via `<iframe>` on a client's existing website. Built with **Blazor Server** on **.NET 10** using **Clean Architecture** and **MudBlazor**.

## Features

- **Service Request Submission** — Full form with contact info, address, issue category/urgency, and description. Supports JSON and XML file upload to pre-fill fields.
- **Inline Confirmation** — After submission, displays a success message with request summary directly on the form.
- **Configurable Branding** — Company name, colors, and logo configurable via `appsettings.json` so each client gets their own look.
- **iframe-Ready** — CORS, `Content-Security-Policy: frame-ancestors`, and `SameSite=None` antiforgery cookies for cross-origin embedding.
- **REST API** — `POST /api/service-requests` and `GET /api/service-requests/{id}` with JSON/XML content negotiation.
- **Swagger/OpenAPI** — Interactive API documentation at `/swagger` in development.
- **ZIP Code Validation** — CSV-based validation using SimpleMaps data (33K+ US ZIP codes).
- **70 Unit & Integration Tests** — Covering domain, application, and infrastructure layers.

## Architecture

The project follows **Clean Architecture** with four layers:

```
ResidentialOpportunity/
├── src/
│   ├── ResidentialOpportunity.Domain/          # Entities, Value Objects, Enums
│   ├── ResidentialOpportunity.Application/     # DTOs, Services, Interfaces, Validators
│   ├── ResidentialOpportunity.Infrastructure/  # EF Core, Repositories, ZIP Validation
│   └── ResidentialOpportunity.Web/             # Blazor Server UI, API Controller
├── tests/
│   ├── ResidentialOpportunity.Domain.Tests/
│   ├── ResidentialOpportunity.Application.Tests/
│   └── ResidentialOpportunity.Infrastructure.Tests/
└── ResidentialOpportunity.slnx
```

### Domain Layer
- **Entities**: `ServiceRequest`
- **Value Objects**: `ContactInfo`, `Address`
- **Enums**: `IssueCategory` (Heating, Cooling, Ventilation, Thermostat, AirQuality, Maintenance, Emergency, Other), `UrgencyLevel` (Low, Standard, Urgent, Emergency), `RequestStatus`

### Application Layer
- **Services**: `ServiceRequestService`
- **DTOs**: `CreateServiceRequestCommand`, `ServiceRequestDto`
- **Validation**: FluentValidation with `CreateServiceRequestValidator`
- **Interfaces**: `IServiceRequestRepository`, `IUnitOfWork`, `IZipCodeValidationService`

### Infrastructure Layer
- **EF Core**: `AppDbContext` with fluent API configurations, owned entity types for value objects, enum-to-string conversions
- **Repositories**: `ServiceRequestRepository`
- **ZIP Validation**: `CsvZipCodeValidationService` — embedded CSV with 33K+ US ZIP codes

### Web Layer (Blazor Server + API)
- **Pages**: SubmitRequest (root `/`) with inline confirmation
- **Layout**: Minimal `MudContainer` — no app bar or nav (designed for iframe embedding)
- **API Controller**: `ServiceRequestsController` with JSON/XML content negotiation
- **Branding**: `BrandingOptions` bound from `appsettings.json`

## Configuration

### appsettings.json

```json
{
  "Branding": {
    "CompanyName": "Your HVAC Company",
    "PrimaryColor": "#1a3a5c",
    "SecondaryColor": "#2980b9",
    "LogoUrl": "https://example.com/logo.png"
  },
  "Iframe": {
    "FrameAncestors": "https://yourclientsite.com",
    "AllowedOrigins": ["https://yourclientsite.com"]
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ResidentialOpportunity;..."
  }
}
```

- **Branding.CompanyName** — Displayed in the form header and page title.
- **Branding.PrimaryColor / SecondaryColor** — MudBlazor theme colors (hex).
- **Branding.LogoUrl** — Optional logo displayed above the company name.
- **Iframe.FrameAncestors** — `Content-Security-Policy: frame-ancestors` value. Use `*` to allow any origin, or a space-separated list of allowed origins.
- **Iframe.AllowedOrigins** — CORS allowed origins array. Empty array allows any origin.

### Embedding via iframe

```html
<iframe src="https://your-resop-instance.com/" width="100%" height="800" frameborder="0"></iframe>
```

## Tech Stack

- .NET 10 / ASP.NET Core 10
- Blazor Server (Interactive Server rendering)
- MudBlazor 9.0.0 (Material Design)
- Entity Framework Core 10 / SQL Server
- FluentValidation 12.1.1
- Swashbuckle.AspNetCore 10.1.4
- xUnit 3.1.4, Moq 4.20.72
- NLog (file logging)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (10.0.100 or later)
- SQL Server (LocalDB or SQL Server Express)

## Getting Started

```bash
git clone https://github.com/<your-username>/ResidentialOpportunity.git
cd ResidentialOpportunity
dotnet build
dotnet test
dotnet run --project src/ResidentialOpportunity.Web
```

The app will be available at `https://localhost:5001` (or the port shown in console output).

On first run, the database is created automatically via `EnsureCreatedAsync()`.

## API Endpoints

All endpoints support JSON and XML via `Accept` and `Content-Type` headers. Swagger UI at `/swagger` in development.

- **`POST /api/service-requests`** — Create a new service request. Returns `201 Created` with `ServiceRequestDto`.
- **`GET /api/service-requests/{id}`** — Get a service request by ID.
- **`GET /api/service-requests?email={email}`** — Lookup requests by email.

## License

This project is proprietary. All rights reserved.
