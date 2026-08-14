using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiBranch.StockTransfer.Domain.Entities;

namespace MultiBranch.StockTransfer.Infrastructure.Persistence.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
     public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");
        builder.HasKey(sm=>sm.Id);

        builder.Property(sm=>sm.ProductId)
            .IsRequired();
        
        builder.Property(sm=>sm.ShelfId)
            .IsRequired();
        
        builder.Property(sm=>sm.EmployeeId)
            .IsRequired();
        
        builder.Property(sm=>sm.Quantity)
            .IsRequired();
        
        builder.Property(sm=>sm.MovementType)
            .IsRequired();
        
        builder.Property(sm=>sm.Reason)
            .HasMaxLength(250);
        
        builder.HasOne(sm=>sm.Product)
            .WithMany(p=>p.StockMovements)
            .HasForeignKey(sm=>sm.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sm=>sm.Shelf)
            .WithMany(sh=>sh.StockMovements)
            .HasForeignKey(sm=>sm.ShelfId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sm=>sm.Employee)
            .WithMany(e=>e.StockMovements)
            .HasForeignKey(sm=>sm.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(sm=>sm.Transfer)
            .WithMany(t=>t.StockMovements)
            .HasForeignKey(sm=>sm.TransferId)
            .OnDelete(DeleteBehavior.Restrict);





    }
}