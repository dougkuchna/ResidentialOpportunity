using Microsoft.EntityFrameworkCore;

namespace ResidentialOpportunity.Infrastructure.Data;

/// <summary>
/// DbContext for legacy tables (dbo.wrkcde, dbo.clnt). Uses the same connection string as AppDbContext.
/// </summary>
public class LegacyDbContext : DbContext
{
    public DbSet<LegacyWorkCode> WorkCodes => Set<LegacyWorkCode>();
    public DbSet<LegacyClient> Clients => Set<LegacyClient>();
    public DbSet<LegacyClientSite> ClientSites => Set<LegacyClientSite>();
    public DbSet<LegacyWebLog> WebLogs => Set<LegacyWebLog>();

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

        modelBuilder.Entity<LegacyClientSite>(e =>
        {
            e.ToTable("clntste", "dbo");
            e.HasKey(s => s.Id);
            e.Property(s => s.Id).HasColumnName("id");
            e.Property(s => s.ClientId).HasColumnName("clnt_id").IsRequired();
            e.Property(s => s.Street).HasColumnName("street").HasMaxLength(300).IsRequired();
            e.Property(s => s.City).HasColumnName("city").HasMaxLength(100).IsRequired();
            e.Property(s => s.State).HasColumnName("state").HasMaxLength(2).IsRequired();
            e.Property(s => s.Zip).HasColumnName("zip").HasMaxLength(10).IsRequired();
            e.Property(s => s.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<LegacyWebLog>(e =>
        {
            e.ToTable("wblg", "dbo");
            e.HasKey(w => w.Id);
            e.Property(w => w.Id).HasColumnName("id");
            e.Property(w => w.ClientId).HasColumnName("clnt_id");
            e.Property(w => w.ClientSiteId).HasColumnName("clntste_id");
            e.Property(w => w.WorkCodeCode).HasColumnName("wrkcde").HasMaxLength(20);
            e.Property(w => w.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            e.Property(w => w.Email).HasColumnName("email").HasMaxLength(254).IsRequired();
            e.Property(w => w.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
            e.Property(w => w.Description).HasColumnName("description").HasMaxLength(2000).IsRequired();
            e.Property(w => w.IssueCategory).HasColumnName("issue_category").HasMaxLength(50).IsRequired();
            e.Property(w => w.Urgency).HasColumnName("urgency").HasMaxLength(20).IsRequired();
            e.Property(w => w.EquipmentDetails).HasColumnName("equipment_details").HasMaxLength(500);
            e.Property(w => w.PreferredSchedule).HasColumnName("preferred_schedule").HasMaxLength(500);
            e.Property(w => w.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
            e.Property(w => w.SubmittedAt).HasColumnName("submitted_at");
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

/// <summary>
/// Maps to dbo.clntste (write).
/// </summary>
public class LegacyClientSite
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string Street { get; set; } = default!;
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public string Zip { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// Maps to dbo.wblg (write).
/// </summary>
public class LegacyWebLog
{
    public Guid Id { get; set; }
    public Guid? ClientId { get; set; }
    public Guid? ClientSiteId { get; set; }
    public string? WorkCodeCode { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string IssueCategory { get; set; } = default!;
    public string Urgency { get; set; } = default!;
    public string? EquipmentDetails { get; set; }
    public string? PreferredSchedule { get; set; }
    public string Status { get; set; } = default!;
    public DateTimeOffset SubmittedAt { get; set; }
}
