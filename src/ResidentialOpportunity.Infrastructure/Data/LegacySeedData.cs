using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ResidentialOpportunity.Infrastructure.Data;

public static class LegacySeedData
{
    /// <summary>
    /// Seeds sample work codes into dbo.wrkcde if the table is empty.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LegacyDbContext>();

        await context.Database.EnsureCreatedAsync().ConfigureAwait(false);

        if (await context.WorkCodes.AnyAsync().ConfigureAwait(false))
            return;

        var workCodes = new List<LegacyWorkCode>
        {
            new() { Code = "ACREPAIR", Description = "AC Repair" },
            new() { Code = "ACINSTALL", Description = "AC Installation" },
            new() { Code = "FURNREP", Description = "Furnace Repair" },
            new() { Code = "FURNINST", Description = "Furnace Installation" },
            new() { Code = "DUCTCLEAN", Description = "Duct Cleaning" },
            new() { Code = "DUCTREPAIR", Description = "Duct Repair" },
            new() { Code = "THERMO", Description = "Thermostat Service" },
            new() { Code = "MAINT", Description = "Preventive Maintenance" },
            new() { Code = "EMERGENCY", Description = "Emergency Service" },
            new() { Code = "AIRQUAL", Description = "Air Quality Assessment" },
            new() { Code = "HEATPUMP", Description = "Heat Pump Service" },
            new() { Code = "VENTINST", Description = "Ventilation Installation" },
            new() { Code = "INSPECT", Description = "System Inspection" },
            new() { Code = "OTHER", Description = "Other Service" }
        };

        await context.WorkCodes.AddRangeAsync(workCodes).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }
}
