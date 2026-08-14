using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiBranch.StockTransfer.Domain.Entities;

namespace MultiBranch.StockTransfer.Infrastructure.Persistence.Configurations;

public class ShelfStockConfiguration : IEntityTypeConfiguration<ShelfStock>
{
     public void Configure(EntityTypeBuilder<ShelfStock> builder)
    {
        builder.ToTable("ShelfStocks");
        builder.HasKey(ss=>ss.Id);
        builder.Property(ss=>ss.ShelfId)
            .IsRequired();  
        
        builder.Property(ss=>ss.ProductId)
            .IsRequired();

        builder.Property(ss=>ss.Quantity)
            .IsRequired();

           
        builder.HasOne(ss=>ss.Shelf)
            .WithMany(sh=>sh.ShelfStocks)
            .HasForeignKey(ss=>ss.ShelfId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ss=>ss.Product)
            .WithMany(p=>p.ShelfStocks)
            .HasForeignKey(ss=>ss.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ss => new { ss.ShelfId, ss.ProductId })
            .IsUnique();
    }
    }