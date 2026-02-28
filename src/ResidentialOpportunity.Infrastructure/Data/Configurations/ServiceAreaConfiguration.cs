using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Infrastructure.Data.Configurations;

public class ServiceAreaConfiguration : IEntityTypeConfiguration<ServiceArea>
{
    public void Configure(EntityTypeBuilder<ServiceArea> builder)
    {
        builder.ToTable("ServiceAreas");

        builder.HasKey(sa => sa.Id);

        builder.Property(sa => sa.ZipCode).HasMaxLength(10).IsRequired();

        builder.HasIndex(sa => sa.ZipCode);
        builder.HasIndex(sa => new { sa.ProviderId, sa.ZipCode }).IsUnique();
    }
}
