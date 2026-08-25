using Microsoft.EntityFrameworkCore;
using MultiBranch.StockTransfer.Application.DTOs.Supplier;
using MultiBranch.StockTransfer.Application.Interfaces;
using MultiBranch.StockTransfer.Domain.Entities;

namespace MultiBranch.StockTransfer.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IApplicationDbContext _context;
    public SupplierService(IApplicationDbContext context)
    {
        _context = context;
    }

      public async Task<List<SupplierDto>> GetAllAsync()
    {
        return await _context.Suppliers
            .AsNoTracking()
            .Where(s => s.IsActive)
            .Select(s => new SupplierDto
            {
               Id=s.Id,
               Email=s.Email,
               Phone=s.Phone,
               CompanyName=s.CompanyName,
               ContactName=s.ContactName,
        })
        .ToListAsync();
    }
   
    public async Task<SupplierDto?> GetByIdAsync(Guid id)
    {
       var supplier =await _context.Suppliers 
        .AsNoTracking()
        .Where(s => s.Id == id && s.IsActive)
        .FirstOrDefaultAsync();
      if(supplier == null)
        {
            return null;
        } 
      return new SupplierDto
      {
          Id=supplier.Id,
          CompanyName=supplier.CompanyName,
          ContactName=supplier.ContactName,
          Phone=supplier.Phone,
          Email=supplier.Email


      };
    } 
     public async  Task<SupplierDto> CreateAsync(CreateSupplierDto dto)
    {
      var supplier = new Supplier
      {
        Email=dto.Email,
        Phone=dto.Phone,
        CompanyName=dto.CompanyName,
        ContactName=dto.ContactName,

      };
      await _context.Suppliers.AddAsync(supplier);
      await _context.SaveChangesAsync();
      
      var result = await GetByIdAsync(supplier.Id);
      if (result == null)
      {
          throw new InvalidOperationException(
        "The supplier was created but could not be retrieved.");

      }
    return result;
    }
    
    public async Task<SupplierDto?> UpdateAsync(Guid id, UpdateSupplierDto dto)
    {
       var supplier = await _context.Suppliers
        .Where(s => s.Id == id && s.IsActive)
        .FirstOrDefaultAsync();

    if (supplier == null)
    {
        return null;
    }
       supplier.CompanyName= dto.CompanyName;
       supplier.ContactName= dto.ContactName;
       supplier.Phone= dto.Phone;
       supplier.Email= dto.Email;
      
       
       await _context.SaveChangesAsync();
       var result = await GetByIdAsync(supplier.Id);
       return result;
    }


     public async Task<bool> DeleteAsync(Guid id)
    {
       var supplier = await _context.Suppliers
       .Where(s => s.Id == id && s.IsActive)
       .FirstOrDefaultAsync();

        if (supplier == null)
    {
        return false;
    }
    supplier.IsActive = false;
    await _context.SaveChangesAsync();

    return true;
    }
   
    

}
