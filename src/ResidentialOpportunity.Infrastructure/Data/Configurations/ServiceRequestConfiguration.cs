using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Infrastructure.Data.Configurations;

public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
{
    public void Configure(EntityTypeBuilder<ServiceRequest> builder)
    {
        builder.ToTable("ServiceRequests");

        builder.HasKey(sr => sr.Id);

        builder.OwnsOne(sr => sr.ContactInfo, ci =>
        {
            ci.Property(c => c.Name).HasColumnName("ContactName").HasMaxLength(200).IsRequired();
            ci.Property(c => c.Email).HasColumnName("ContactEmail").HasMaxLength(254).IsRequired();
            ci.Property(c => c.Phone).HasColumnName("ContactPhone").HasMaxLength(20).IsRequired();
        });

        builder.OwnsOne(sr => sr.Address, a =>
        {
            a.Property(x => x.Street).HasColumnName("Street").HasMaxLength(300).IsRequired();
            a.Property(x => x.City).HasColumnName("City").HasMaxLength(100).IsRequired();
            a.Property(x => x.State).HasColumnName("State").HasMaxLength(2).IsRequired();
            a.Property(x => x.ZipCode).HasColumnName("ZipCode").HasMaxLength(10).IsRequired();
            a.HasIndex(x => x.ZipCode);
        });

        builder.Property(sr => sr.IssueDescription).HasMaxLength(2000).IsRequired();
        builder.Property(sr => sr.IssueCategory).HasConversion<string>().HasMaxLength(50);
        builder.Property(sr => sr.UrgencyLevel).HasConversion<string>().HasMaxLength(20);
        builder.Property(sr => sr.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(sr => sr.EquipmentDetails).HasMaxLength(500);
        builder.Property(sr => sr.PreferredSchedule).HasMaxLength(200);

        builder.HasIndex(sr => sr.CustomerId);
        builder.HasIndex(sr => sr.CreatedAt);
    }
}
