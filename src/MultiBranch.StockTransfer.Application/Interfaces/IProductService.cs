using MultiBranch.StockTransfer.Application.DTOs.Product;

namespace MultiBranch.StockTransfer.Application.Interfaces;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync();

    Task<ProductDto?> GetByIdAsync(Guid id);

    Task<ProductDto> CreateAsync(CreateProductDto dto);

    Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto);

    Task<bool> DeleteAsync(Guid id);
}