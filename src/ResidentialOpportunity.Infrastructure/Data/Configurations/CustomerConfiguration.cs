using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResidentialOpportunity.Domain.Entities;

namespace ResidentialOpportunity.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.IdentityUserId).HasMaxLength(450).IsRequired();
        builder.HasIndex(c => c.IdentityUserId).IsUnique();

        builder.OwnsOne(c => c.ContactInfo, ci =>
        {
            ci.Property(x => x.Name).HasColumnName("ContactName").HasMaxLength(200).IsRequired();
            ci.Property(x => x.Email).HasColumnName("ContactEmail").HasMaxLength(254).IsRequired();
            ci.Property(x => x.Phone).HasColumnName("ContactPhone").HasMaxLength(20).IsRequired();
        });

        builder.OwnsOne(c => c.Address, a =>
        {
            a.Property(x => x.Street).HasColumnName("Street").HasMaxLength(300);
            a.Property(x => x.City).HasColumnName("City").HasMaxLength(100);
            a.Property(x => x.State).HasColumnName("State").HasMaxLength(2);
            a.Property(x => x.ZipCode).HasColumnName("ZipCode").HasMaxLength(10);
        });
    }
}
