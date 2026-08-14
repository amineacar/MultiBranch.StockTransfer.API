using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiBranch.StockTransfer.Domain.Entities;

namespace MultiBranch.StockTransfer.Infrastructure.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
     public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");
        builder.HasKey(sup=>sup.Id);
        builder.Property(sup=>sup.CompanyName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(sup=>sup.ContactName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(sup=>sup.Phone)
            .IsRequired()
            .HasMaxLength(20);
        builder.HasIndex(sup=>sup.Phone)
            .IsUnique();

        builder.Property(sup=>sup.Email)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(sup=>sup.Email)   
            .IsUnique();

        builder.HasMany(sup=>sup.Products)
            .WithOne(p=>p.Supplier)
            .HasForeignKey(p=>p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

    }
    }