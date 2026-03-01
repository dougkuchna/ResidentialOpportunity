using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Infrastructure.Data;
using ResidentialOpportunity.Infrastructure.Data.Repositories;

namespace ResidentialOpportunity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
        services.AddScoped<IHvacProviderRepository, HvacProviderRepository>();

        // ZIP code validation (CSV-based, loaded once into memory)
        services.AddSingleton<IZipCodeValidationService, CsvZipCodeValidationService>();

        return services;
    }
}
