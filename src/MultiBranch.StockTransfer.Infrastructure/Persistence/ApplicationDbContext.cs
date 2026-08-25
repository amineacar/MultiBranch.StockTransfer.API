using MultiBranch.StockTransfer.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MultiBranch.StockTransfer.Domain.Entities;



namespace MultiBranch.StockTransfer.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Shelf> Shelves => Set<Shelf>();
    public DbSet<ShelfStock> ShelfStocks => Set<ShelfStock>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Store> Stores =>  Set<Store>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<TransferItem> TransferItems => Set<TransferItem>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
       
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
{
    return await Database.BeginTransactionAsync();
}

}