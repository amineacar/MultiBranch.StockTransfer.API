using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiBranch.StockTransfer.Domain.Entities;

namespace MultiBranch.StockTransfer.Infrastructure.Persistence.Configurations;

public class TransferItemConfiguration : IEntityTypeConfiguration<TransferItem>
{
     public void Configure(EntityTypeBuilder<TransferItem> builder)
    {
        builder.ToTable("TransferItems");
        builder.HasKey(ti=>ti.Id);
        builder.Property(ti=>ti.TransferId)
            .IsRequired();
        
        builder.Property(ti=>ti.ProductId)
            .IsRequired();

        builder.Property(ti=>ti.SourceShelfId)
            .IsRequired();


        builder.Property(ti=>ti.Quantity)
            .IsRequired();
         
        
        builder.HasOne(ti => ti.Transfer)
       .WithMany(t => t.TransferItems)
       .HasForeignKey(ti => ti.TransferId)
       .OnDelete(DeleteBehavior.Restrict);

       builder.HasOne(ti => ti.Product)
       .WithMany()
       .HasForeignKey(ti => ti.ProductId)
       .OnDelete(DeleteBehavior.Restrict);

       builder.HasOne(ti => ti.SourceShelf)
       .WithMany()
       .HasForeignKey(ti => ti.SourceShelfId)
       .OnDelete(DeleteBehavior.Restrict);

       builder.HasOne(ti => ti.TargetShelf)
       .WithMany()
       .HasForeignKey(ti => ti.TargetShelfId)
       .OnDelete(DeleteBehavior.Restrict);
        
    }
}