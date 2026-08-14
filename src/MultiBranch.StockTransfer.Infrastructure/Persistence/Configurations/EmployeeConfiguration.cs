using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiBranch.StockTransfer.Domain.Entities;

namespace MultiBranch.StockTransfer.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
      builder.ToTable("Employees");
      builder.HasKey(e=>e.Id);
      builder.Property(e=>e.FirstName)
            .IsRequired()
            .HasMaxLength(50);

      builder.Property(e=>e.LastName)
            .IsRequired()
            .HasMaxLength(50);

      builder.Property(e=>e.Email)
            .IsRequired()
            .HasMaxLength(100);
      builder.HasIndex(e=>e.Email)
            .IsUnique();

      builder.Property(e=>e.EmployeeCode)
            .IsRequired()
            .HasMaxLength(20);
      builder.HasIndex(e=>e.EmployeeCode)
            .IsUnique();

      builder.HasOne(e=>e.Store)
            .WithMany(s=>s.Employees)
            .HasForeignKey(e=>e.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
            
    }
}