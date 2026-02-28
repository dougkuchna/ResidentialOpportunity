# Changelog

All notable changes to the ResidentialOpportunity project are documented here.

## [0.7.0] - 2026-02-28

### MudBlazor UI Integration
- Installed MudBlazor 9.0.0 NuGet package
- Configured MudServices, MudThemeProvider (primary #1a3a5c, secondary #2980b9), MudPopoverProvider, MudDialogProvider, MudSnackbarProvider
- Rewrote MainLayout.razor with MudLayout, MudAppBar, MudDrawer (Mini variant with OpenMiniOnHover), MudMainContent
- Rewrote NavMenu.razor with MudNavMenu and MudNavLink (Material icons)
- Rewrote Home.razor with MudPaper, MudGrid, MudCard, MudButton, MudList
- Rewrote SubmitRequest.razor markup with MudTextField, MudSelect, MudGrid, MudPaper, MudAlert, MudButton
- Rewrote FindProviders.razor with MudTextField (search adornment), MudCard grid, MudAlert
- Rewrote RequestConfirmation.razor with MudAlert, MudSimpleTable, MudChip, MudCard, MudButton
- Rewrote MyRequests.razor with MudCard, MudChip, MudProgressCircular, MudButton
- Updated Register/Login/Logout pages with MudPaper/MudText/MudAlert wrappers (native HTML forms preserved for static SSR)
- Cleaned up app.css: removed 400+ lines of custom styles replaced by MudBlazor, kept only Blazor framework and auth form styles
- Replaced MainLayout.razor.css and NavMenu.razor.css scoped styles (MudBlazor handles layout/nav)
- Added Roboto font and MudBlazor CSS/JS to App.razor

## [0.6.0] - 2026-02-28

### Phase 6: Authentication & Account Management
- Integrated ASP.NET Core Identity with `IdentityDbContext` (shared database)
- Created Register page (static SSR): email, password, name, phone → creates IdentityUser + Customer entity
- Created Login page (static SSR): email/password auth with "Remember Me"
- Created Logout page with confirmation
- Created MyRequests page (`[Authorize]`): shows authenticated user's service requests
- Updated NavMenu with `AuthorizeView`: shows My Requests/Sign Out when authenticated, Login/Register when anonymous
- Updated SubmitRequest to auto-fill contact info and link customerId for authenticated users
- Added `ClaimRequestsForCustomerAsync` to ServiceRequestService: claims anonymous requests by matching email on register/login
- Identity registration in Program.cs: password policy (8+ chars, upper/lower/digit), unique email required
- Cookie auth: 14-day expiry, sliding expiration, HttpOnly

## [0.5.0] - 2026-02-28

### Phase 5: Web Layer — API Controllers
- Created `ServiceRequestsController` with POST (create), GET by ID, and GET by email endpoints
- Created `ProvidersController` with GET by ZIP code endpoint
- Full JSON and XML content negotiation via `[Produces]` and `[Consumes]` attributes
- FluentValidation errors returned as RFC 7807 `ValidationProblemDetails`
- Swagger/OpenAPI documentation for all endpoints

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
