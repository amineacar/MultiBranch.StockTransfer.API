using MultiBranch.StockTransfer.Application.DTOs.ShelfStock;

namespace MultiBranch.StockTransfer.Application.Interfaces;

public interface IShelfStockService
{
    Task<List<ShelfStockDto>> GetAllAsync();

    Task<ShelfStockDto?> GetByIdAsync(Guid id);

    Task<ShelfStockDto> CreateAsync(CreateShelfStockDto dto);

    Task<bool> DeleteAsync(Guid id);

    Task<StockOperationResultDto> RemoveStockAsync(
        Guid shelfId,
        Guid productId,
        int quantity);

    Task<ShelfStockDto> AddStockAsync(
        Guid shelfId,
        Guid productId,
        int quantity);

    Task<ShelfStockDto> StockInAsync(
        StockMovementOperationDto dto);

    Task<StockOperationResultDto> StockOutAsync(
        StockMovementOperationDto dto);

    Task<bool> RelocateAsync(
        Guid sourceShelfId,
        Guid targetShelfId,
        Guid productId,
        Guid employeeId,
        int quantity,
        string reason);
      Task<StockOperationResultDto> TransferOutAsync(
        Guid shelfId,
        Guid productId,
        Guid employeeId,
        Guid transferId,
        int quantity);

    Task<ShelfStockDto> TransferInAsync(
        Guid shelfId,
        Guid productId,
        Guid employeeId,
        Guid transferId,
        int quantity,
        string reason);
}