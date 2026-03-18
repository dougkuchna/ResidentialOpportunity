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

        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Email).HasMaxLength(254).IsRequired();
        builder.Property(c => c.MobilePhone).HasMaxLength(20).IsRequired();
        builder.Property(c => c.PreferredContactMethod).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.CustomerType).HasConversion<string>().HasMaxLength(20);

        builder.OwnsOne(c => c.Address, a =>
        {
            a.Property(x => x.Street).HasColumnName("Street").HasMaxLength(300).IsRequired();
            a.Property(x => x.City).HasColumnName("City").HasMaxLength(100).IsRequired();
            a.Property(x => x.State).HasColumnName("State").HasMaxLength(2).IsRequired();
            a.Property(x => x.ZipCode).HasColumnName("ZipCode").HasMaxLength(10).IsRequired();
        });

        builder.HasIndex(c => c.Email);
    }
}
