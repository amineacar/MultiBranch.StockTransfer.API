using Microsoft.EntityFrameworkCore;
using MultiBranch.StockTransfer.Application.DTOs.Store;
using MultiBranch.StockTransfer.Application.Interfaces;
using MultiBranch.StockTransfer.Domain.Entities;

namespace MultiBranch.StockTransfer.Application.Services;

public class StoreService : IStoreService
{
    private readonly IApplicationDbContext _context;

    public StoreService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StoreDto>> GetAllAsync()
    {
        return await _context.Stores
            .AsNoTracking()
            .Where(s => s.IsActive)
            .Select(s => new StoreDto
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                Code = s.Code
            })
            .ToListAsync();
    }

    public async Task<StoreDto?> GetByIdAsync(Guid id)
    {
        var store = await _context.Stores
            .AsNoTracking()
            .Where(s => s.Id == id && s.IsActive)
            .FirstOrDefaultAsync();

        if (store == null)
        {
            return null;
        }

        return new StoreDto
        {
            Id = store.Id,
            Name = store.Name,
            Address = store.Address,
            Code = store.Code
        };
    }

    public async Task<StoreDto> CreateAsync(CreateStoreDto dto)
    {
        var store = new Store
        {
            Name = dto.Name,
            Address = dto.Address,
            Code = dto.Code
        };

        await _context.Stores.AddAsync(store);
        await _context.SaveChangesAsync();

        var result = await GetByIdAsync(store.Id);

        if (result == null)
        {
            throw new InvalidOperationException(
                "The store was created but could not be retrieved."
            );
        }

        return result;
        }

    public async Task<StoreDto?> UpdateAsync(
        Guid id,
        UpdateStoreDto dto)
    {
        var store = await _context.Stores
            .Where(s => s.Id == id && s.IsActive)
            .FirstOrDefaultAsync();

        if (store == null)
        {
            return null;
        }

        store.Name = dto.Name;
        store.Address = dto.Address;
        store.Code = dto.Code;

        await _context.SaveChangesAsync();

        var result = await GetByIdAsync(store.Id);

        return result;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var store = await _context.Stores
            .Where(s => s.Id == id && s.IsActive)
            .FirstOrDefaultAsync();

        if (store == null)
        {
            return false;
        }

        store.IsActive = false;

        await _context.SaveChangesAsync();

        return true;
    }
}