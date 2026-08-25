using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MultiBranch.StockTransfer.Application.DTOs.Transfer;
using MultiBranch.StockTransfer.Application.DTOs.TransferItem;
using MultiBranch.StockTransfer.Application.Interfaces;
using MultiBranch.StockTransfer.Domain.Entities;
using MultiBranch.StockTransfer.Domain.Enums;

namespace MultiBranch.StockTransfer.Application.Services;

public class TransferService : ITransferService
{
    private readonly IApplicationDbContext _context;
    private readonly IShelfStockService _shelfStockService;
    private readonly IValidator<CreateTransferDto> _createTransferValidator;

    public TransferService(
        IApplicationDbContext context,
        IShelfStockService shelfStockService,
        IValidator<CreateTransferDto> createTransferValidator)
        => (_context, _shelfStockService, _createTransferValidator) =
            (context, shelfStockService, createTransferValidator);

    public Task<List<TransferDto>> GetAllAsync() => _context.Transfers.AsNoTracking()
        .Where(t => t.IsActive).Select(t => new TransferDto
        {
            Id = t.Id, SourceStoreId = t.SourceStoreId, TargetStoreId = t.TargetStoreId,
            EmployeeId = t.EmployeeId, Status = t.Status
        }).ToListAsync();

    public Task<TransferDto?> GetByIdAsync(Guid id) => _context.Transfers.AsNoTracking()
        .Where(t => t.Id == id && t.IsActive).Select(t => new TransferDto
        {
            Id = t.Id, SourceStoreId = t.SourceStoreId, TargetStoreId = t.TargetStoreId,
            EmployeeId = t.EmployeeId, Status = t.Status,
            TransferItems = t.TransferItems.Select(item => new TransferItemDto
            {
                Id = item.Id, ProductId = item.ProductId, SourceShelfId = item.SourceShelfId,
                TargetShelfId = item.TargetShelfId, Quantity = item.Quantity
            }).ToList()
        }).FirstOrDefaultAsync();
      public async Task<TransferDto> CreateAsync(CreateTransferDto dto)
    {
        await _createTransferValidator.ValidateAndThrowAsync(dto);
        await ValidateCreateAsync(dto);
        var result = await ExecuteInTransactionAsync(async () =>
        {
            var transfer = new Transfer
            {
                SourceStoreId = dto.SourceStoreId, TargetStoreId = dto.TargetStoreId,
                EmployeeId = dto.EmployeeId, Status = TransferStatus.InTransit,
                TransferItems = dto.TransferItems.Select(item => new TransferItem
                {
                    ProductId = item.ProductId, SourceShelfId = item.SourceShelfId,
                    TargetShelfId = item.TargetShelfId, Quantity = item.Quantity
                }).ToList()
            };
            await _context.Transfers.AddAsync(transfer);
            var warnings = await ProcessStockAsync(transfer.TransferItems, transfer.Id, dto.EmployeeId, StockMovementType.TransferOut, "Transfer", item => item.SourceShelfId);
            await _context.SaveChangesAsync();
            return (TransferId: transfer.Id,Warnings:warnings); 
        });

        var transferDto = (await GetByIdAsync(result.TransferId))!;
        transferDto.Warnings.AddRange(result.Warnings);
        return transferDto;
    }

    public Task<TransferDto?> CompleteAsync(Guid id, Guid employeeId) 
    => UpdateTransferStatusAsync(id, employeeId, TransferStatus.Completed);
    public Task<TransferDto?> CancelAsync(Guid id, Guid employeeId)
     => UpdateTransferStatusAsync(id, employeeId, TransferStatus.Cancelled);

    private async Task<TransferDto?> UpdateTransferStatusAsync(Guid id, Guid employeeId, TransferStatus status)
    {
        var transferId = await ExecuteInTransactionAsync(async () =>
        {
            var transfer = await GetActiveInTransitTransferAsync(id);
            if (transfer is null) return (Guid?)null;

            var employeeStoreId = await GetEmployeeStoreIdAsync(employeeId);
            if (status == TransferStatus.Completed)
            {
                if (employeeStoreId != transfer.TargetStoreId)
                    throw new InvalidOperationException(
                        "Only an employee from the target store can complete the transfer.");
                if (transfer.TransferItems.Any(item => !item.TargetShelfId.HasValue))
                    throw new InvalidOperationException("Target shelf is required to complete the transfer.");

                await ValidateShelvesAsync(transfer.TransferItems.Select(item => item.TargetShelfId!.Value), transfer.TargetStoreId, "target");
                await ProcessStockAsync(transfer.TransferItems, transfer.Id, employeeId,
                    StockMovementType.TransferIn, "Transfer completed", item => item.TargetShelfId!.Value);
            }
            else
            {
                if (employeeStoreId != transfer.SourceStoreId && employeeStoreId != transfer.TargetStoreId)
                    throw new InvalidOperationException(
                        "Only employees from the source or target store can cancel the transfer.");

                await ProcessStockAsync(transfer.TransferItems, transfer.Id, employeeId,
                    StockMovementType.TransferIn, "Transfer cancelled", item => item.SourceShelfId);
            }

            transfer.Status = status;
            await _context.SaveChangesAsync();
            return transfer.Id;
        });

        return transferId.HasValue ? await GetByIdAsync(transferId.Value) : null;
    }

    private async Task ValidateCreateAsync(CreateTransferDto dto)
    {
        if (!await _context.Stores.AnyAsync(store => store.Id == dto.SourceStoreId && store.IsActive))
            throw new InvalidOperationException("Source store not found.");
        if (!await _context.Stores.AnyAsync(store => store.Id == dto.TargetStoreId && store.IsActive))
            throw new InvalidOperationException("Target store not found.");
        if (await GetEmployeeStoreIdAsync(dto.EmployeeId) != dto.SourceStoreId)
            throw new InvalidOperationException(
                "Employee can only start a transfer from their own store.");

        var productIds = dto.TransferItems.Select(item => item.ProductId).Distinct().ToList();
        if (await _context.Products.CountAsync(product => productIds.Contains(product.Id) && product.IsActive) != productIds.Count)
            throw new InvalidOperationException("One or more products were not found.");

        await ValidateShelvesAsync(dto.TransferItems.Select(item => item.SourceShelfId), dto.SourceStoreId, "source");
        await ValidateShelvesAsync(dto.TransferItems.Select(item => item.TargetShelfId!.Value), dto.TargetStoreId, "target");
    }

    private async Task<Transfer?> GetActiveInTransitTransferAsync(Guid id)
    {
        var transfer = await _context.Transfers.Include(t => t.TransferItems)
            .SingleOrDefaultAsync(t => t.Id == id && t.IsActive);

        if (transfer is null) return null;
        if (transfer.Status != TransferStatus.InTransit)
            throw new InvalidOperationException("Only transfers in transit can be processed.");
        if (transfer.TransferItems.Count == 0)
            throw new InvalidOperationException("Transfer must contain at least one item.");

        return transfer;
    }

    private async Task ValidateShelvesAsync(IEnumerable<Guid> shelfIds, Guid storeId, string side)
    {
        var ids = shelfIds.Distinct().ToList();
        var shelves = await _context.Shelves.Where(s => ids.Contains(s.Id) && s.IsActive)
            .Select(s => new { s.Id, s.StoreId }).ToListAsync();

        if (shelves.Count != ids.Count)
            throw new InvalidOperationException($"One or more {side} shelves were not found.");
        if (shelves.Any(shelf => shelf.StoreId != storeId))
            throw new InvalidOperationException($"{char.ToUpper(side[0])}{side[1..]} shelf does not belong to its store.");
    }

    private async Task<Guid> GetEmployeeStoreIdAsync(Guid employeeId)
    {
        var storeId = await _context.Employees.Where(e => e.Id == employeeId && e.IsActive)
            .Select(e => (Guid?)e.StoreId).SingleOrDefaultAsync();
        return storeId ?? throw new InvalidOperationException("Employee not found.");
    }
      private async Task<List<string>> ProcessStockAsync(
    IEnumerable<TransferItem> items,
    Guid transferId,
    Guid employeeId,
    StockMovementType movementType,
    string reason,
    Func<TransferItem, Guid> shelfId)
{
    var warnings = new List<string>();

    var stockChanges = items
        .GroupBy(item => new { ShelfId = shelfId(item), item.ProductId })
        .Select(group => new
        {
            group.Key.ShelfId,
            group.Key.ProductId,
            Quantity = group.Sum(item => item.Quantity)
        });

    foreach (var change in stockChanges)
    {
        if (movementType == StockMovementType.TransferOut)
        {
            var result = await _shelfStockService.TransferOutAsync(
                change.ShelfId,
                change.ProductId,
                employeeId,
                transferId,
                change.Quantity);

            warnings.AddRange(result.Warnings);
        }
        else
        {
            await _shelfStockService.TransferInAsync(
                change.ShelfId,
                change.ProductId,
                employeeId,
                transferId,
                change.Quantity,
                reason);
        }
    }

    return warnings;
}
       private async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        await using var transaction = await _context.BeginTransactionAsync();
        try
        {
            var result = await action();
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
