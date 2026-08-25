using Microsoft.EntityFrameworkCore;
using MultiBranch.StockTransfer.Application.DTOs.Product;
using MultiBranch.StockTransfer.Application.Interfaces;
using MultiBranch.StockTransfer.Domain.Entities;

namespace MultiBranch.StockTransfer.Application.Services;

public class ProductService : IProductService
{
    private readonly IApplicationDbContext _context;
    public ProductService(IApplicationDbContext context)
    {
        _context = context;
    }

      public async Task<List<ProductDto>> GetAllAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .Select(p => new ProductDto
            {
               Id=p.Id,
               Name=p.Name,
               Barcode=p.Barcode,
               MinimumStockLevel=p.MinimumStockLevel,
               CategoryName=p.Category.Name,
               SupplierName=p.Supplier.CompanyName,
        })
        .ToListAsync();
    }
   
     public async Task<ProductDto?> GetByIdAsync(Guid id)
{
    return await _context.Products
        .AsNoTracking()
        .Where(p => p.Id == id && p.IsActive)
        .Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Barcode = p.Barcode,
            MinimumStockLevel = p.MinimumStockLevel,
            CategoryName = p.Category.Name,
            SupplierName = p.Supplier.CompanyName
        })
        .FirstOrDefaultAsync();
}
    
     public async  Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
      var product = new Product
      {
       Name=dto.Name,
       Barcode=dto.Barcode,
       MinimumStockLevel=dto.MinimumStockLevel,
       CategoryId=dto.CategoryId,
       SupplierId=dto.SupplierId

      };
      await _context.Products.AddAsync(product);
      await _context.SaveChangesAsync();
      
      var result = await GetByIdAsync(product.Id);
      if (result == null)
      {
          throw new InvalidOperationException( "The product was created but could not be retrieved.");
      }
    return result;
    }
    
    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto)
    {
       var product = await _context.Products
        .Where(p => p.Id == id && p.IsActive)
        .FirstOrDefaultAsync();

    if (product == null)
    {
        return null;
    }
       product.Name= dto.Name;
       product.Barcode= dto.Barcode;
       product.MinimumStockLevel= dto.MinimumStockLevel;
       product.CategoryId= dto.CategoryId;
       product.SupplierId= dto.SupplierId;
       
       await _context.SaveChangesAsync();
       var result = await GetByIdAsync(product.Id);
       return result;
    }


     public async Task<bool> DeleteAsync(Guid id)
    {
       var product = await _context.Products
       .Where(p => p.Id == id && p.IsActive)
       .FirstOrDefaultAsync();

        if (product == null)
    {
        return false;
    }
    product.IsActive = false;
    await _context.SaveChangesAsync();

    return true;
    }
   
    

}
