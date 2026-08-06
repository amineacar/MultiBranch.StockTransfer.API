using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiBranch.StockTransfer.Domain.Entities;

namespace MultiBranch.StockTransfer.Infrastructure.Persistence.Configurations;

public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
{
     public void Configure(EntityTypeBuilder<Transfer> builder)
    {
        builder.ToTable("Transfers");
        builder.HasKey(t=>t.Id);
        builder.Property(t=>t.SourceStoreId)
            .IsRequired();
            

        builder.Property(t=>t.TargetStoreId)
            .IsRequired();
           
        
        builder.Property(t=>t.EmployeeId)
            .IsRequired();
           
        
        builder.Property(t=>t.Status)
            .IsRequired();
        
        builder.HasOne(t=>t.SourceStore)
            .WithMany(s=>s.TransfersFrom)
            .HasForeignKey(t=>t.SourceStoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t=>t.TargetStore)
            .WithMany(t=>t.TransfersTo)
            .HasForeignKey(t=>t.TargetStoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t=>t.Employee)
            .WithMany(e=>e.Transfers)
            .HasForeignKey(t=>t.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(t => t.TransferItems)
            .WithOne(ti => ti.Transfer)
            .HasForeignKey(ti => ti.TransferId)
            .OnDelete(DeleteBehavior.Restrict);

       builder.HasMany(t => t.StockMovements)
            .WithOne(sm => sm.Transfer)
            .HasForeignKey(sm => sm.TransferId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}