using MultiBranch.StockTransfer.Application.DTOs.Store;

namespace MultiBranch.StockTransfer.Application.Interfaces;

public interface IStoreService
{
    Task<List<StoreDto>> GetAllAsync();

    Task<StoreDto?> GetByIdAsync(Guid id);

    Task<StoreDto> CreateAsync(CreateStoreDto dto);

    Task<StoreDto?> UpdateAsync(Guid id, UpdateStoreDto dto);

    Task<bool> DeleteAsync(Guid id);
}