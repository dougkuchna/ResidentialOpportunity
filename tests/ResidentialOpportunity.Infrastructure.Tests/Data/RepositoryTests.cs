using Microsoft.EntityFrameworkCore;
using ResidentialOpportunity.Domain.Entities;
using ResidentialOpportunity.Domain.Enums;
using ResidentialOpportunity.Domain.ValueObjects;
using ResidentialOpportunity.Infrastructure.Data;
using ResidentialOpportunity.Infrastructure.Data.Repositories;

namespace ResidentialOpportunity.Infrastructure.Tests.Data;

public class RepositoryTests : IDisposable
{
    private readonly AppDbContext _context;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    // --- ServiceRequestRepository ---

    [Fact]
    public async Task ServiceRequest_AddAndRetrieveById()
    {
        var repo = new ServiceRequestRepository(_context);
        var entity = ServiceRequest.Create(
            new ContactInfo("John Doe", "john@example.com", "555-1234"),
            new Address("123 Main St", "Springfield", "IL", "62704"),
            "AC not cooling", IssueCategory.Cooling, UrgencyLevel.Standard);

        await repo.AddAsync(entity);
        await _context.SaveChangesAsync();

        var retrieved = await repo.GetByIdAsync(entity.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(entity.Id, retrieved!.Id);
        Assert.Equal("John Doe", retrieved.ContactInfo.Name);
        Assert.Equal("AC not cooling", retrieved.IssueDescription);
    }

    [Fact]
    public async Task ServiceRequest_GetByEmail_ReturnsMatching()
    {
        var repo = new ServiceRequestRepository(_context);
        var entity1 = ServiceRequest.Create(
            new ContactInfo("John", "john@test.com", "555-0001"),
            new Address("1 St", "City", "IL", "60601"),
            "Issue 1", IssueCategory.Cooling, UrgencyLevel.Standard);
        var entity2 = ServiceRequest.Create(
            new ContactInfo("John", "john@test.com", "555-0001"),
            new Address("2 St", "City", "IL", "60601"),
            "Issue 2", IssueCategory.Heating, UrgencyLevel.Urgent);
        var entity3 = ServiceRequest.Create(
            new ContactInfo("Jane", "jane@other.com", "555-9999"),
            new Address("3 St", "City", "IL", "60601"),
            "Issue 3", IssueCategory.Other, UrgencyLevel.Standard);

        await repo.AddAsync(entity1);
        await repo.AddAsync(entity2);
        await repo.AddAsync(entity3);
        await _context.SaveChangesAsync();

        var results = await repo.GetByEmailAsync("john@test.com");

        Assert.Equal(2, results.Count);
        Assert.All(results, r => Assert.Equal("john@test.com", r.ContactInfo.Email));
    }

    [Fact]
    public async Task ServiceRequest_GetByCustomerId_ReturnsMatching()
    {
        var repo = new ServiceRequestRepository(_context);
        var customerId = Guid.NewGuid();
        var entity = ServiceRequest.Create(
            new ContactInfo("Test", "test@test.com", "555-0000"),
            new Address("1 St", "City", "IL", "60601"),
            "Issue", IssueCategory.Other, UrgencyLevel.Standard,
            customerId: customerId);

        await repo.AddAsync(entity);
        await _context.SaveChangesAsync();

        var results = await repo.GetByCustomerIdAsync(customerId);

        Assert.Single(results);
        Assert.Equal(customerId, results[0].CustomerId);
    }

    [Fact]
    public async Task ServiceRequest_GetById_NotFound_ReturnsNull()
    {
        var repo = new ServiceRequestRepository(_context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    // --- HvacProviderRepository ---

    [Fact]
    public async Task HvacProvider_AddAndRetrieveByZipCode()
    {
        var repo = new HvacProviderRepository(_context);
        var provider = HvacProvider.Create(
            "Cool Air", "555-1111", "info@coolair.com",
            new Address("100 Blvd", "Springfield", "IL", "62704"));
        provider.AddServiceArea("62704");
        provider.AddServiceArea("62705");

        await repo.AddAsync(provider);
        await _context.SaveChangesAsync();

        var results = await repo.GetByZipCodeAsync("62704");

        Assert.Single(results);
        Assert.Equal("Cool Air", results[0].CompanyName);
    }

    [Fact]
    public async Task HvacProvider_GetByZipCode_OnlyReturnsActive()
    {
        var repo = new HvacProviderRepository(_context);

        var active = HvacProvider.Create(
            "Active HVAC", "555-1111", "active@test.com",
            new Address("1 St", "City", "IL", "60601"));
        active.AddServiceArea("60601");

        var inactive = HvacProvider.Create(
            "Inactive HVAC", "555-2222", "inactive@test.com",
            new Address("2 St", "City", "IL", "60601"));
        inactive.AddServiceArea("60601");
        inactive.Deactivate();

        await repo.AddAsync(active);
        await repo.AddAsync(inactive);
        await _context.SaveChangesAsync();

        var results = await repo.GetByZipCodeAsync("60601");

        Assert.Single(results);
        Assert.Equal("Active HVAC", results[0].CompanyName);
    }

    [Fact]
    public async Task HvacProvider_GetByZipCode_NoMatch_ReturnsEmpty()
    {
        var repo = new HvacProviderRepository(_context);
        var provider = HvacProvider.Create(
            "HVAC Co", "555-1111", "info@test.com",
            new Address("1 St", "City", "IL", "60601"));
        provider.AddServiceArea("60601");

        await repo.AddAsync(provider);
        await _context.SaveChangesAsync();

        var results = await repo.GetByZipCodeAsync("99999");

        Assert.Empty(results);
    }

    [Fact]
    public async Task HvacProvider_MultipleProvidersForSameZip()
    {
        var repo = new HvacProviderRepository(_context);

        var p1 = HvacProvider.Create("Alpha HVAC", "555-1111", "a@test.com",
            new Address("1 St", "City", "IL", "62704"));
        p1.AddServiceArea("62704");

        var p2 = HvacProvider.Create("Beta HVAC", "555-2222", "b@test.com",
            new Address("2 St", "City", "IL", "62704"));
        p2.AddServiceArea("62704");

        await repo.AddAsync(p1);
        await repo.AddAsync(p2);
        await _context.SaveChangesAsync();

        var results = await repo.GetByZipCodeAsync("62704");

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task HvacProvider_GetAllActive_ExcludesInactive()
    {
        var repo = new HvacProviderRepository(_context);

        var active = HvacProvider.Create("Active", "555-1111", "a@test.com",
            new Address("1 St", "City", "IL", "60601"));
        var inactive = HvacProvider.Create("Inactive", "555-2222", "b@test.com",
            new Address("2 St", "City", "IL", "60601"));
        inactive.Deactivate();

        await repo.AddAsync(active);
        await repo.AddAsync(inactive);
        await _context.SaveChangesAsync();

        var results = await repo.GetAllActiveAsync();

        Assert.Single(results);
        Assert.Equal("Active", results[0].CompanyName);
    }
}
