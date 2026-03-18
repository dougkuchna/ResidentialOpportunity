using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ResidentialOpportunity.Infrastructure.Data;

public static class LegacySeedData
{
    /// <summary>
    /// Ensures legacy tables exist and seeds sample work codes.
    /// Uses raw SQL because EnsureCreatedAsync is a no-op when the database already exists.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LegacyDbContext>();

        // Create legacy tables if they don't exist
        await EnsureLegacyTablesAsync(context).ConfigureAwait(false);

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

    private static async Task EnsureLegacyTablesAsync(LegacyDbContext context)
    {
        const string sql = """
            IF OBJECT_ID('dbo.wrkcde', 'U') IS NULL
            CREATE TABLE dbo.wrkcde (
                code   VARCHAR(20)  NOT NULL PRIMARY KEY,
                description VARCHAR(200) NOT NULL
            );

            IF OBJECT_ID('dbo.clnt', 'U') IS NULL
            CREATE TABLE dbo.clnt (
                id                UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                name              VARCHAR(200)     NOT NULL,
                email             VARCHAR(254)     NOT NULL,
                street            VARCHAR(300)     NOT NULL,
                city              VARCHAR(100)     NOT NULL,
                state             VARCHAR(2)       NOT NULL,
                zip               VARCHAR(10)      NOT NULL,
                mobile_phone      VARCHAR(20)      NOT NULL,
                preferred_contact VARCHAR(20)      NOT NULL,
                client_type       VARCHAR(20)      NOT NULL,
                created_at        DATETIMEOFFSET   NOT NULL
            );

            IF OBJECT_ID('dbo.clntste', 'U') IS NULL
            CREATE TABLE dbo.clntste (
                id         UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                clnt_id    UNIQUEIDENTIFIER NOT NULL,
                street     VARCHAR(300)     NOT NULL,
                city       VARCHAR(100)     NOT NULL,
                state      VARCHAR(2)       NOT NULL,
                zip        VARCHAR(10)      NOT NULL,
                created_at DATETIMEOFFSET   NOT NULL
            );

            IF OBJECT_ID('dbo.wblg', 'U') IS NULL
            CREATE TABLE dbo.wblg (
                id                 UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                clnt_id            UNIQUEIDENTIFIER NULL,
                clntste_id         UNIQUEIDENTIFIER NULL,
                wrkcde             VARCHAR(20)      NULL,
                name               VARCHAR(200)     NOT NULL,
                email              VARCHAR(254)     NOT NULL,
                phone              VARCHAR(20)      NOT NULL,
                description        NVARCHAR(2000)   NOT NULL,
                issue_category     VARCHAR(50)      NOT NULL,
                urgency            VARCHAR(20)      NOT NULL,
                equipment_details  VARCHAR(500)     NULL,
                preferred_schedule VARCHAR(500)     NULL,
                status             VARCHAR(20)      NOT NULL,
                submitted_at       DATETIMEOFFSET   NOT NULL
            );
            """;

        await context.Database.ExecuteSqlRawAsync(sql).ConfigureAwait(false);
    }
}
