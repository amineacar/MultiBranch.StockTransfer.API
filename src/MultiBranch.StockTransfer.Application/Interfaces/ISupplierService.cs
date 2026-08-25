using MultiBranch.StockTransfer.Application.DTOs.Supplier;

namespace MultiBranch.StockTransfer.Application.Interfaces;

public interface ISupplierService
{
    Task<List<SupplierDto>> GetAllAsync();

    Task<SupplierDto?> GetByIdAsync(Guid id);

    Task<SupplierDto> CreateAsync(CreateSupplierDto dto);

    Task<SupplierDto?> UpdateAsync(Guid id, UpdateSupplierDto dto);

    Task<bool> DeleteAsync(Guid id);
}