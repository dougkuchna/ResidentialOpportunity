using Microsoft.EntityFrameworkCore;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Infrastructure.Data;

public class AppDbContext : DbContext, IUnitOfWork
{
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
