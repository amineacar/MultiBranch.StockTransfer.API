using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiBranch.StockTransfer.Domain.Entities;

namespace MultiBranch.StockTransfer.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
     public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p=>p.Id);

        builder.Property(p=>p.Name)
           .IsRequired()
           .HasMaxLength(150);

        builder.Property(p=>p.Barcode)
             .IsRequired()
             .HasMaxLength(50);
        builder.HasIndex(p=>p.Barcode)
             .IsUnique();

        builder.Property(p=>p.MinimumStockLevel)
             .IsRequired();
      
        builder.HasOne(p=>p.Category)
             .WithMany(c => c.Products)
             .HasForeignKey(p=>p.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Supplier)
             .WithMany(sup => sup.Products)
             .HasForeignKey(p=>p.SupplierId)
             .OnDelete(DeleteBehavior.Restrict);
         
        builder.HasMany(p=>p.ShelfStocks)
             .WithOne(ss=>ss.Product)
             .HasForeignKey(ss=>ss.ProductId)
             .OnDelete(DeleteBehavior.Restrict);


    }
}