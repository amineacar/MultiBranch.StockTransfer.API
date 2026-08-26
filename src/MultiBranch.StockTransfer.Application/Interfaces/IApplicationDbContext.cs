using Microsoft.EntityFrameworkCore;
using MultiBranch.StockTransfer.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;


namespace MultiBranch.StockTransfer.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<Employee> Employees { get; }
    DbSet<Shelf> Shelves { get; }
    DbSet<Transfer> Transfers { get; }
    DbSet<ShelfStock> ShelfStocks { get; }
    DbSet<StockMovement> StockMovements { get; }
    DbSet<Store> Stores { get; }
    DbSet<TransferItem> TransferItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync();
}