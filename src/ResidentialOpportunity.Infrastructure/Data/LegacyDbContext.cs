using Microsoft.EntityFrameworkCore;

namespace ResidentialOpportunity.Infrastructure.Data;

/// <summary>
/// DbContext for legacy tables (dbo.wrkcde, dbo.clnt). Uses the same connection string as AppDbContext.
/// </summary>
public class LegacyDbContext : DbContext
{
    public DbSet<LegacyWorkCode> WorkCodes => Set<LegacyWorkCode>();
    public DbSet<LegacyClient> Clients => Set<LegacyClient>();

    public LegacyDbContext(DbContextOptions<LegacyDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LegacyWorkCode>(e =>
        {
            e.ToTable("wrkcde", "dbo");
            e.HasKey(w => w.Code);
            e.Property(w => w.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
            e.Property(w => w.Description).HasColumnName("description").HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<LegacyClient>(e =>
        {
            e.ToTable("clnt", "dbo");
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasColumnName("id");
            e.Property(c => c.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            e.Property(c => c.Email).HasColumnName("email").HasMaxLength(254).IsRequired();
            e.Property(c => c.Street).HasColumnName("street").HasMaxLength(300).IsRequired();
            e.Property(c => c.City).HasColumnName("city").HasMaxLength(100).IsRequired();
            e.Property(c => c.State).HasColumnName("state").HasMaxLength(2).IsRequired();
            e.Property(c => c.Zip).HasColumnName("zip").HasMaxLength(10).IsRequired();
            e.Property(c => c.MobilePhone).HasColumnName("mobile_phone").HasMaxLength(20).IsRequired();
            e.Property(c => c.PreferredContact).HasColumnName("preferred_contact").HasMaxLength(20).IsRequired();
            e.Property(c => c.ClientType).HasColumnName("client_type").HasMaxLength(20).IsRequired();
            e.Property(c => c.CreatedAt).HasColumnName("created_at");
        });
    }
}

/// <summary>
/// Maps to dbo.wrkcde (read-only).
/// </summary>
public class LegacyWorkCode
{
    public string Code { get; set; } = default!;
    public string Description { get; set; } = default!;
}

/// <summary>
/// Maps to dbo.clnt (write).
/// </summary>
public class LegacyClient
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Street { get; set; } = default!;
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public string Zip { get; set; } = default!;
    public string MobilePhone { get; set; } = default!;
    public string PreferredContact { get; set; } = default!;
    public string ClientType { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
}
