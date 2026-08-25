using MultiBranch.StockTransfer.Application.DTOs.Shelf;

namespace MultiBranch.StockTransfer.Application.Interfaces;

public interface IShelfService
{
    Task<List<ShelfDto>> GetAllAsync();

    Task<ShelfDto?> GetByIdAsync(Guid id);

    Task<ShelfDto> CreateAsync(CreateShelfDto dto);

    Task<ShelfDto?> UpdateAsync(Guid id, UpdateShelfDto dto);

    Task<bool> DeleteAsync(Guid id);
}