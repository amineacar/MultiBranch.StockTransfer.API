using Microsoft.EntityFrameworkCore;
using MultiBranch.StockTransfer.Application.DTOs.Category;
using MultiBranch.StockTransfer.Application.Interfaces;
using MultiBranch.StockTransfer.Domain.Entities;

namespace MultiBranch.StockTransfer.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IApplicationDbContext _context;
    public CategoryService(IApplicationDbContext context)
    {
        _context = context;
    }
     public async Task<List<CategoryDto>> GetAllAsync()
    {
          return await _context.Categories 
            .Where(c => c.IsActive)
            .Select(c => new CategoryDto
            {
               Id=c.Id,
               Name=c.Name,
               Description=c.Description
        })
        .ToListAsync();
    }

     public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        var category =await _context.Categories 
        .Where(c => c.Id == id && c.IsActive)
        .FirstOrDefaultAsync();
      if(category == null)
        {
            return null;
        } 
      return new CategoryDto
      {
          Id= category.Id,
          Name=category.Name,
          Description =category.Description

      };
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
          var category = new Category
      {
        Name=dto.Name,
        Description=dto.Description

      };
      await _context.Categories.AddAsync(category);
      await _context.SaveChangesAsync();
      
      var result = await GetByIdAsync(category.Id);
      if (result == null)
      {
         throw new InvalidOperationException(
            "The category was created but could not be retrieved."
        );
      }
    return result;
    }

     public async Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryDto dto)
    {
         var category = await _context.Categories 
        .Where(c => c.Id == id && c.IsActive)
        .FirstOrDefaultAsync();

    if (category == null)
    {
        return null;
    }
       category.Name= dto.Name;
       category.Description=dto.Description;
       
       await _context.SaveChangesAsync();
       var result = await GetByIdAsync(category.Id);
       return result;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _context.Categories 
       .Where(c=> c.Id == id && c.IsActive)
       .FirstOrDefaultAsync();

        if (category == null)
    {
        return false;
    }
    category.IsActive = false;
    await _context.SaveChangesAsync();

    return true;
    }


   
}