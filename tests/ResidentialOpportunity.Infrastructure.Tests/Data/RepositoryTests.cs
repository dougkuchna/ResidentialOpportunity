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
    public async Task ServiceRequest_GetById_NotFound_ReturnsNull()
    {
        var repo = new ServiceRequestRepository(_context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

}
