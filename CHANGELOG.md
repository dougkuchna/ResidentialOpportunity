# Changelog

All notable changes to the ResidentialOpportunity project are documented here.

## [0.4.0] - 2026-02-28

### Phase 4: Web Layer — Blazor UI
- Created `SubmitRequest.razor` with EditForm, FluentValidation, JSON/XML file upload via InputFile
- Created `FindProviders.razor` with ZIP code search and provider card display
- Created `RequestConfirmation.razor` showing request summary and auto-matched providers
- Updated `Home.razor` with landing page featuring action cards and common issue list
- Updated `MainLayout.razor` with sidebar + main-content layout
- Created `NavMenu.razor` with sidebar navigation links
- Added comprehensive CSS for layout, forms, buttons, alerts, cards, and confirmation views
- Updated `_Imports.razor` with Application and Domain namespaces

### Phase 8: Testing (completed alongside earlier phases)
- 86 tests passing: domain, application, and infrastructure layers
- Domain tests: value objects (Address, ContactInfo), entities (ServiceRequest, HvacProvider)
- Application tests: validator rules, ServiceRequestService, ProviderLookupService (with Moq)
- Infrastructure tests: EF Core InMemory integration tests for repositories

## [0.3.0] - 2026-02-28

### Phase 3: Infrastructure Layer
- Implemented `AppDbContext` with `IUnitOfWork` support
- Created entity configurations using Fluent API (owned types for value objects, enum-to-string)
- Implemented `ServiceRequestRepository` and `HvacProviderRepository`
- Added `SeedData.cs` with 5 sample HVAC providers across IL and TX
- Created `DependencyInjection.cs` for infrastructure service registration

## [0.2.0] - 2026-02-28

### Phase 2: Application Layer
- Defined repository interfaces: `IServiceRequestRepository`, `IHvacProviderRepository`, `IUnitOfWork`
- Created DTOs: `CreateServiceRequestCommand`, `ServiceRequestDto`, `ProviderSearchResult`
- Implemented `ServiceRequestService` (create, get by ID) and `ProviderLookupService` (search by ZIP)
- Added `CreateServiceRequestValidator` with FluentValidation (name, email, phone, ZIP, description required)
- Created mapping extensions for entity ↔ DTO conversion

## [0.1.0] - 2026-02-28

### Phase 1: Project Scaffolding
- Created solution with `.slnx` format (new .NET 10 XML-based)
- Set up 4 source projects: Domain, Application, Infrastructure, Web
- Set up 3 test projects: Domain.Tests, Application.Tests, Infrastructure.Tests
- Configured project references following Clean Architecture dependency rules
- Implemented domain entities: `ServiceRequest`, `Customer`, `HvacProvider`, `ServiceArea`
- Implemented value objects: `ContactInfo`, `Address`
- Defined enums: `IssueCategory`, `UrgencyLevel`, `RequestStatus`
