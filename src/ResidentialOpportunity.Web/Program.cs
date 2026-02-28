using FluentValidation;
using ResidentialOpportunity.Application.Services;
using ResidentialOpportunity.Application.Validators;
using ResidentialOpportunity.Infrastructure;
using ResidentialOpportunity.Infrastructure.Data;
using ResidentialOpportunity.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// API controllers with XML + JSON support
builder.Services.AddControllers()
    .AddXmlSerializerFormatters();

// OpenAPI / Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure (EF Core, repositories)
builder.Services.AddInfrastructure(builder.Configuration);

// Application services
builder.Services.AddValidatorsFromAssemblyContaining<CreateServiceRequestValidator>();
builder.Services.AddScoped<ServiceRequestService>();
builder.Services.AddScoped<ProviderLookupService>();

var app = builder.Build();

// Seed sample data
await SeedData.InitializeAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
