using Microsoft.EntityFrameworkCore;
using MultiBranch.StockTransfer.Application.DTOs.ShelfStock;
using MultiBranch.StockTransfer.Application.Interfaces;
using MultiBranch.StockTransfer.Domain.Entities;
using MultiBranch.StockTransfer.Domain.Enums;

namespace MultiBranch.StockTransfer.Application.Services;

public class ShelfStockService : IShelfStockService
{
    private readonly IApplicationDbContext _context;

    public ShelfStockService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShelfStockDto>> GetAllAsync()
    {
        return await _context.ShelfStocks
            .Where(s => s.IsActive)
            .AsNoTracking()
            .Select(s => new ShelfStockDto
            {
                Id = s.Id,
                ShelfCode = s.Shelf.Code,
                ProductName = s.Product.Name,
                Quantity = s.Quantity
            })
            .ToListAsync();
    }

    public async Task<ShelfStockDto?> GetByIdAsync(Guid id)
    {
        var shelfStock = await _context.ShelfStocks
        .AsNoTracking()
        .Include(s => s.Shelf)
        .Include(s => s.Product)
        .Where(s => s.Id == id && s.IsActive)
        .FirstOrDefaultAsync();

        if (shelfStock == null)
        {
            return null;
        }

        return new ShelfStockDto
        {
            Id = shelfStock.Id,
            ShelfCode = shelfStock.Shelf.Code,
            ProductName = shelfStock.Product.Name,
            Quantity = shelfStock.Quantity
        };
    }

    public async Task<ShelfStockDto> CreateAsync(CreateShelfStockDto dto)
    {
    var shelf = await _context.Shelves
        .Where(s => s.Id == dto.ShelfId && s.IsActive)
        .FirstOrDefaultAsync();

    if (shelf == null)
    {
        throw new InvalidOperationException("Shelf not found.");
    }
    var product = await _context.Products
        .Where(p => p.Id == dto.ProductId && p.IsActive)
        .FirstOrDefaultAsync();

    if (product == null)
    {
        throw new InvalidOperationException("Product not found.");
    }

    var existingShelfStock = await _context.ShelfStocks
        .Where(s => s.ShelfId == dto.ShelfId && s.ProductId == dto.ProductId && s.IsActive)
        .FirstOrDefaultAsync();

    if (existingShelfStock != null)
    {
        throw new InvalidOperationException("This product already exists on the shelf.");
    }

    var shelfStock = new ShelfStock
    {
        ShelfId = dto.ShelfId,
        ProductId = dto.ProductId
    };

    await _context.ShelfStocks.AddAsync(shelfStock);
    await _context.SaveChangesAsync();

    var result = await GetByIdAsync(shelfStock.Id);
    if (result == null)
    {
        throw new InvalidOperationException("The shelf stock was created but could not be retrieved.");
    }

    return result;
    }   

    public async Task<bool> DeleteAsync(Guid id)
    {
        var shelfStock = await _context.ShelfStocks
            .Where(s => s.Id == id && s.IsActive)
            .FirstOrDefaultAsync();

        if (shelfStock == null)
        {
            return false;
        }
        shelfStock.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<StockOperationResultDto> RemoveStockAsync(Guid shelfId, Guid productId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new InvalidOperationException("Quantity must be greater than zero.");
        }

        var shelfStock = await _context.ShelfStocks
            .Include(s => s.Shelf)
            .Include(s => s.Product)
            .Where(s => s.ShelfId == shelfId && s.ProductId == productId && s.IsActive)
            .FirstOrDefaultAsync();

        if (shelfStock == null)
        {
            throw new InvalidOperationException("Product stock was not found on the shelf.");
        }

        if (shelfStock.Quantity < quantity)
        {
            throw new InvalidOperationException("Not enough stock to remove the requested quantity.");
        }

        shelfStock.Quantity -= quantity;

        var result = new StockOperationResultDto
    {
        ShelfStock = new ShelfStockDto
        {
            Id = shelfStock.Id,
            ShelfCode = shelfStock.Shelf.Code,
            ProductName = shelfStock.Product.Name,
            Quantity = shelfStock.Quantity
        }
    };

    if (shelfStock.Quantity < shelfStock.Product.MinimumStockLevel)
    {
        result.Warnings.Add(
            $"{shelfStock.Product.Name} stock on shelf " +
            $"{shelfStock.Shelf.Code} is below the minimum stock level.");
    }
    await _context.SaveChangesAsync();
    return result;   
    }

   public async Task<ShelfStockDto> AddStockAsync( Guid shelfId,Guid productId,int quantity)
{
    if (quantity <= 0)
    {
        throw new InvalidOperationException("Quantity must be greater than zero.");
    }
    var shelfStock = await _context.ShelfStocks
        .Include(s => s.Shelf)
        .Include(s => s.Product)
        .Where(s =>
            s.ShelfId == shelfId &&
            s.ProductId == productId &&
            s.IsActive)
        .FirstOrDefaultAsync();

    if (shelfStock == null)
    {
        throw new InvalidOperationException("Product stock was not found on the shelf.");
    }
    var currentTotalQuantity = await _context.ShelfStocks
        .Where(s => s.ShelfId == shelfId && s.IsActive)
        .SumAsync(s => s.Quantity);

    if (currentTotalQuantity + quantity > shelfStock.Shelf.Capacity)
    {
        throw new InvalidOperationException("Adding this quantity would exceed the shelf capacity.");
    }
    shelfStock.Quantity += quantity;
    await _context.SaveChangesAsync();
    return new ShelfStockDto
    {
        Id = shelfStock.Id,
        ShelfCode = shelfStock.Shelf.Code,
        ProductName = shelfStock.Product.Name,
        Quantity = shelfStock.Quantity
    };
}
    private async Task<Guid> GetEmployeeStoreIdAsync(Guid employeeId)
{
    var storeId = await _context.Employees
        .Where(e => e.Id == employeeId && e.IsActive)
        .Select(e => (Guid?)e.StoreId)
        .SingleOrDefaultAsync();

    return storeId ?? throw new InvalidOperationException("Employee not found.");
}

    private async Task ValidateEmployeeShelfAsync( Guid employeeId, Guid shelfId)
{
    var employeeStoreId = await GetEmployeeStoreIdAsync(employeeId);
    var shelfStoreId = await _context.Shelves
        .Where(s => s.Id == shelfId && s.IsActive)
        .Select(s => (Guid?)s.StoreId)
        .SingleOrDefaultAsync();
    if (!shelfStoreId.HasValue)
    {
        throw new InvalidOperationException("Shelf not found.");
    }
    if (employeeStoreId != shelfStoreId.Value)
    {
        throw new InvalidOperationException(
            "Employee can only operate on shelves of their own store.");
    }}

    public async Task<ShelfStockDto> StockInAsync(StockMovementOperationDto dto){
    if (dto.MovementType != StockMovementType.StockIn)
    {
        throw new InvalidOperationException("StockIn operation must use StockIn movement type.");
    }
    if (string.IsNullOrWhiteSpace(dto.Reason))
    {
        throw new InvalidOperationException("Reason is required.");
    }

    await ValidateEmployeeShelfAsync( dto.EmployeeId, dto.ShelfId);
    await using var transaction = await _context.BeginTransactionAsync();
    try
    {
        var result = await AddStockAsync(dto.ShelfId,dto.ProductId,dto.Quantity);

        var movement = new StockMovement
        {
            ProductId = dto.ProductId,
            ShelfId = dto.ShelfId,
            EmployeeId = dto.EmployeeId,
            Quantity = dto.Quantity,
            MovementType = StockMovementType.StockIn,
            Reason = dto.Reason
        };

        await _context.StockMovements.AddAsync(movement);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return result;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }}
    public async Task<StockOperationResultDto> TransferOutAsync(Guid shelfId,Guid productId,Guid employeeId,Guid transferId,int quantity)
{
    if (quantity <= 0)
    {
        throw new InvalidOperationException("Quantity must be greater than zero.");
    }
    await ValidateEmployeeShelfAsync(employeeId,shelfId);
    var result = await RemoveStockAsync( shelfId,productId,quantity);

    var movement = new StockMovement
    {
        ProductId = productId,
        ShelfId = shelfId,
        EmployeeId = employeeId,
        TransferId = transferId,
        Quantity = quantity,
        MovementType = StockMovementType.TransferOut,
        Reason = "Transfer started"
    };

    await _context.StockMovements.AddAsync(movement);
    return result;
}

     public async Task<ShelfStockDto> TransferInAsync(Guid shelfId,Guid productId,Guid employeeId,Guid transferId,int quantity,string reason)
{
    if (quantity <= 0)
    {
        throw new InvalidOperationException("Quantity must be greater than zero.");
    }

    if (string.IsNullOrWhiteSpace(reason))
    {
        throw new InvalidOperationException("Reason is required.");
    }

    await ValidateEmployeeShelfAsync(employeeId,shelfId);
    var result = await AddStockAsync(shelfId,productId,quantity);

    var movement = new StockMovement
    {
        ProductId = productId,
        ShelfId = shelfId,
        EmployeeId = employeeId,
        TransferId = transferId,
        Quantity = quantity,
        MovementType = StockMovementType.TransferIn,
        Reason = reason
    };

    await _context.StockMovements.AddAsync(movement);
    return result;
}
     
    public async Task<StockOperationResultDto> StockOutAsync(StockMovementOperationDto dto)
{
    if (dto.MovementType != StockMovementType.Sale && dto.MovementType != StockMovementType.Waste)
    {
        throw new InvalidOperationException("Stock out operation only supports Sale or Waste.");
    }

    if (string.IsNullOrWhiteSpace(dto.Reason))
    {
        throw new InvalidOperationException("Reason is required.");
    }
    await ValidateEmployeeShelfAsync(dto.EmployeeId, dto.ShelfId);

    await using var transaction = await _context.BeginTransactionAsync();
    try
    {
        var result = await RemoveStockAsync(dto.ShelfId,dto.ProductId,dto.Quantity);

        var movement = new StockMovement
        {
            ProductId = dto.ProductId,
            ShelfId = dto.ShelfId,
            EmployeeId = dto.EmployeeId,
            Quantity = dto.Quantity,
            MovementType = dto.MovementType,
            Reason = dto.Reason
        };

        await _context.StockMovements.AddAsync(movement);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return result;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }}
    public async Task<bool> RelocateAsync(Guid sourceShelfId,Guid targetShelfId,Guid productId,Guid employeeId, int quantity,string reason)
    {
    if (sourceShelfId == targetShelfId)
    {
        throw new InvalidOperationException("Source and target shelves must be different.");
    }
    if (quantity <= 0)
    {
        throw new InvalidOperationException("Quantity must be greater than zero.");
    }
    if (string.IsNullOrWhiteSpace(reason))
    {
        throw new InvalidOperationException("Reason is required.");
    }
    await ValidateEmployeeShelfAsync( employeeId, sourceShelfId);
    await ValidateEmployeeShelfAsync( employeeId,targetShelfId);

    await using var transaction = await _context.BeginTransactionAsync();
    try
    {
        await RemoveStockAsync(sourceShelfId,productId,quantity);
        await AddStockAsync(targetShelfId,productId,quantity);

        var movements = new[]
        {
            new StockMovement
            {
                ProductId = productId,
                ShelfId = sourceShelfId,
                EmployeeId = employeeId,
                Quantity = quantity,
                MovementType = StockMovementType.RelocationOut,
                Reason = reason
            },

            new StockMovement
            {
                ProductId = productId,
                ShelfId = targetShelfId,
                EmployeeId = employeeId,
                Quantity = quantity,
                MovementType = StockMovementType.RelocationIn,
                Reason = reason
            }
        };

        await _context.StockMovements.AddRangeAsync(movements);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return true;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
}
