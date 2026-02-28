using FluentValidation;
using Microsoft.AspNetCore.Identity;
using MudBlazor.Services;
using ResidentialOpportunity.Application.Services;
using ResidentialOpportunity.Application.Validators;
using ResidentialOpportunity.Infrastructure;
using ResidentialOpportunity.Infrastructure.Data;
using ResidentialOpportunity.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// MudBlazor
builder.Services.AddMudServices();

// API controllers with XML + JSON support
builder.Services.AddControllers()
    .AddXmlSerializerFormatters();

// OpenAPI / Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure (EF Core, repositories)
builder.Services.AddInfrastructure(builder.Configuration);

// ASP.NET Core Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ResidentialOpportunity.Infrastructure.Data.AppDbContext>()
.AddDefaultTokenProviders();

// Cookie auth configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath = "/account/login";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

// Application services
builder.Services.AddValidatorsFromAssemblyContaining<CreateServiceRequestValidator>();
builder.Services.AddScoped<ServiceRequestService>();
builder.Services.AddScoped<ProviderLookupService>();

builder.Services.AddCascadingAuthenticationState();

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

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
