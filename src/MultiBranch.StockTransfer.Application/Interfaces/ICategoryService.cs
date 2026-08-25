using MultiBranch.StockTransfer.Application.DTOs.Category;

namespace MultiBranch.StockTransfer.Application.Interfaces;
   
 public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(Guid Id);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<CategoryDto?> UpdateAsync(Guid Id, UpdateCategoryDto dto);
    Task<bool> DeleteAsync(Guid Id);

}