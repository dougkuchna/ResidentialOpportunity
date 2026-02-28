using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResidentialOpportunity.Application.Interfaces;
using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>, IUnitOfWork
{
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<HvacProvider> HvacProviders => Set<HvacProvider>();
    public DbSet<ServiceArea> ServiceAreas => Set<ServiceArea>();
    public DbSet<Customer> Customers => Set<Customer>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
