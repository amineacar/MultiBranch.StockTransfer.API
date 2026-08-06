using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiBranch.StockTransfer.Domain.Entities;

namespace MultiBranch.StockTransfer.Infrastructure.Persistence.Configurations;

public class ShelfConfiguration : IEntityTypeConfiguration<Shelf>
{
     public void Configure(EntityTypeBuilder<Shelf> builder)
    {
        builder.ToTable("Shelves");
        builder.HasKey(sh=>sh.Id);
        builder.Property(sh=>sh.Code)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(sh=>sh.Capacity)
            .IsRequired();

        builder.HasOne(sh=>sh.Store)
            .WithMany(s=>s.Shelves)
            .HasForeignKey(sh=>sh.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.StoreId, s.Code })
            .IsUnique();

        builder.HasMany(sh=>sh.ShelfStocks)
            .WithOne(ss=>ss.Shelf)
            .HasForeignKey(ss=>ss.ShelfId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
    }
}