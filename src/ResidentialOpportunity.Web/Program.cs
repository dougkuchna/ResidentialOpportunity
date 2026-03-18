using FluentValidation;
using MudBlazor.Services;
using NLog;
using NLog.Web;
using ResidentialOpportunity.Application.Services;
using ResidentialOpportunity.Application.Validators;
using ResidentialOpportunity.Infrastructure;
using ResidentialOpportunity.Web.Components;
using ResidentialOpportunity.Web.Configuration;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // NLog: Setup NLog for file logging (Warning+)
    builder.Host.UseNLog();

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    // Detailed circuit errors in development
    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddServerSideBlazor().AddCircuitOptions(o => o.DetailedErrors = true);
    }

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

    // Branding configuration
    builder.Services.Configure<BrandingOptions>(
        builder.Configuration.GetSection(BrandingOptions.SectionName));

    // Application services
    builder.Services.AddValidatorsFromAssemblyContaining<CreateServiceRequestValidator>();
    builder.Services.AddScoped<ServiceRequestService>();

    // CORS for iframe embedding
    var allowedOrigins = builder.Configuration.GetSection("Iframe:AllowedOrigins").Get<string[]>() ?? [];
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            if (allowedOrigins.Length > 0)
                policy.WithOrigins(allowedOrigins);
            else
                policy.AllowAnyOrigin();

            policy.AllowAnyHeader().AllowAnyMethod();
        });
    });

    // Antiforgery: SameSite=None for cross-origin iframe
    builder.Services.AddAntiforgery(options =>
    {
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    var app = builder.Build();

    // Ensure database is created
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ResidentialOpportunity.Infrastructure.Data.AppDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

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

    // iframe embedding headers
    var frameAncestors = builder.Configuration["Iframe:FrameAncestors"] ?? "*";
    app.Use(async (context, next) =>
    {
        context.Response.Headers["Content-Security-Policy"] = $"frame-ancestors {frameAncestors}";
        context.Response.Headers.Remove("X-Frame-Options");
        await next();
    });

    app.UseHttpsRedirection();
    app.UseCors();
    app.UseAntiforgery();

    app.MapStaticAssets();
    app.MapControllers();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.Run();
}
catch (OperationCanceledException)
{
    // Expected during Ctrl+C / SIGINT shutdown — not an error
}
catch (Exception ex)
{
    logger.Error(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    LogManager.Shutdown();
}
