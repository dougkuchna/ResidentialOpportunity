using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ResidentialOpportunity.Domain.Entities;
using ResidentialOpportunity.Domain.ValueObjects;

namespace ResidentialOpportunity.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.EnsureCreatedAsync();

        if (await context.HvacProviders.AnyAsync())
            return; // Already seeded

        var providers = CreateSampleProviders();
        await context.HvacProviders.AddRangeAsync(providers);
        await context.SaveChangesAsync();
    }

    private static List<HvacProvider> CreateSampleProviders()
    {
        var providers = new List<HvacProvider>();

        // Provider 1 — Springfield, IL area
        var coolAir = HvacProvider.Create(
            "Cool Air Solutions",
            "217-555-0101",
            "service@coolairsolutions.com",
            new Address("100 Industrial Blvd", "Springfield", "IL", "62704"),
            website: "https://coolairsolutions.com",
            description: "Full-service residential HVAC. Heating, cooling, and ventilation specialists serving central Illinois since 2005.");
        coolAir.AddServiceArea("62701");
        coolAir.AddServiceArea("62702");
        coolAir.AddServiceArea("62703");
        coolAir.AddServiceArea("62704");
        coolAir.AddServiceArea("62707");
        providers.Add(coolAir);

        // Provider 2 — Springfield, IL area
        var heatWave = HvacProvider.Create(
            "HeatWave Comfort Systems",
            "217-555-0202",
            "info@heatwavecomfort.com",
            new Address("250 Commerce Dr", "Springfield", "IL", "62703"),
            website: "https://heatwavecomfort.com",
            description: "Emergency HVAC repair and installation. 24/7 service available. Licensed and insured.");
        heatWave.AddServiceArea("62702");
        heatWave.AddServiceArea("62703");
        heatWave.AddServiceArea("62704");
        heatWave.AddServiceArea("62711");
        providers.Add(heatWave);

        // Provider 3 — Chicago, IL area
        var windyCityHvac = HvacProvider.Create(
            "Windy City HVAC Pros",
            "312-555-0303",
            "support@windycityhvac.com",
            new Address("500 Michigan Ave", "Chicago", "IL", "60601"),
            website: "https://windycityhvac.com",
            description: "Chicago's trusted HVAC contractor. Furnace repair, AC installation, duct cleaning, and indoor air quality.");
        windyCityHvac.AddServiceArea("60601");
        windyCityHvac.AddServiceArea("60602");
        windyCityHvac.AddServiceArea("60603");
        windyCityHvac.AddServiceArea("60604");
        windyCityHvac.AddServiceArea("60605");
        windyCityHvac.AddServiceArea("60606");
        providers.Add(windyCityHvac);

        // Provider 4 — Chicago suburbs
        var suburbComfort = HvacProvider.Create(
            "Suburban Comfort Heating & Air",
            "630-555-0404",
            "hello@suburbancomfort.com",
            new Address("1200 Butterfield Rd", "Downers Grove", "IL", "60515"),
            website: "https://suburbancomfort.com",
            description: "Serving Chicago's western suburbs. Residential and light commercial HVAC. Free estimates on new installations.");
        suburbComfort.AddServiceArea("60515");
        suburbComfort.AddServiceArea("60516");
        suburbComfort.AddServiceArea("60517");
        suburbComfort.AddServiceArea("60514");
        suburbComfort.AddServiceArea("60532");
        providers.Add(suburbComfort);

        // Provider 5 — Dallas, TX area
        var loneStarAir = HvacProvider.Create(
            "Lone Star Air Conditioning",
            "214-555-0505",
            "service@lonestarair.com",
            new Address("800 Elm St", "Dallas", "TX", "75201"),
            website: "https://lonestarair.com",
            description: "Dallas-Fort Worth HVAC experts. AC repair, heat pump installation, and seasonal maintenance plans.");
        loneStarAir.AddServiceArea("75201");
        loneStarAir.AddServiceArea("75202");
        loneStarAir.AddServiceArea("75203");
        loneStarAir.AddServiceArea("75204");
        loneStarAir.AddServiceArea("75205");
        loneStarAir.AddServiceArea("75206");
        providers.Add(loneStarAir);

        return providers;
    }
}
