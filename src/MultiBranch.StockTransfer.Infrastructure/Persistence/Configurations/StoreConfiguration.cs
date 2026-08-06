using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiBranch.StockTransfer.Domain.Entities;

namespace MultiBranch.StockTransfer.Infrastructure.Persistence.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    
    public void Configure(EntityTypeBuilder<Store> builder)

    {
        builder.ToTable("Stores");
        builder.HasKey(s=>s.Id);
        builder.Property(s=>s.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s=>s.Address)
            .IsRequired()
            .HasMaxLength(250);
        
        builder.Property(s=>s.Code)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(s=>s.Code)
            .IsUnique();

        
        builder.HasMany(s=>s.Employees)
            .WithOne(e=>e.Store)
            .HasForeignKey(e=>e.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s=>s.Shelves)
           .WithOne(sh=>sh.Store)
           .HasForeignKey(sh=>sh.StoreId)
           .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(s => s.TransfersFrom)
           .WithOne(t => t.SourceStore)
           .HasForeignKey(t => t.SourceStoreId)
           .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.TransfersTo)
           .WithOne(t => t.TargetStore)
           .HasForeignKey(t => t.TargetStoreId)
           .OnDelete(DeleteBehavior.Restrict);

    }
}
