using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Infrastructure.Data.Configurations;

public class HvacProviderConfiguration : IEntityTypeConfiguration<HvacProvider>
{
    public void Configure(EntityTypeBuilder<HvacProvider> builder)
    {
        builder.ToTable("HvacProviders");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.CompanyName).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Phone).HasMaxLength(20).IsRequired();
        builder.Property(p => p.Email).HasMaxLength(254).IsRequired();
        builder.Property(p => p.Website).HasMaxLength(500);
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.LogoUrl).HasMaxLength(500);

        builder.OwnsOne(p => p.Address, a =>
        {
            a.Property(x => x.Street).HasColumnName("Street").HasMaxLength(300).IsRequired();
            a.Property(x => x.City).HasColumnName("City").HasMaxLength(100).IsRequired();
            a.Property(x => x.State).HasColumnName("State").HasMaxLength(2).IsRequired();
            a.Property(x => x.ZipCode).HasColumnName("ZipCode").HasMaxLength(10).IsRequired();
        });

        builder.HasMany(p => p.ServiceAreas)
            .WithOne(sa => sa.Provider)
            .HasForeignKey(sa => sa.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.IsActive);

        // Access the backing field for ServiceAreas collection
        builder.Navigation(p => p.ServiceAreas)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_serviceAreas");
    }
}
