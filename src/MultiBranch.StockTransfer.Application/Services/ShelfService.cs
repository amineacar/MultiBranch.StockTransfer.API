using Microsoft.EntityFrameworkCore;
using MultiBranch.StockTransfer.Application.DTOs.Shelf;
using MultiBranch.StockTransfer.Application.Interfaces;
using MultiBranch.StockTransfer.Domain.Entities;

namespace MultiBranch.StockTransfer.Application.Services;

public class ShelfService : IShelfService
{
    private readonly IApplicationDbContext _context;

    public ShelfService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShelfDto>> GetAllAsync()
    {
        return await _context.Shelves
            .Where(s => s.IsActive)
            .AsNoTracking()
            .Select(s => new ShelfDto
            {
                Id = s.Id,
                Code = s.Code,
                Capacity = s.Capacity,
                StoreName = s.Store.Name
            })
            .ToListAsync();
    }

    public async Task<ShelfDto?> GetByIdAsync(Guid id)
{
    return await _context.Shelves
        .AsNoTracking()
        .Where(s => s.Id == id && s.IsActive)
        .Select(s => new ShelfDto
        {
            Id = s.Id,
            Code = s.Code,
            Capacity = s.Capacity,
            StoreName = s.Store.Name
        })
        .FirstOrDefaultAsync();
}

    public async Task<ShelfDto> CreateAsync(CreateShelfDto dto)
    {
        var shelf = new Shelf
        {
            Code = dto.Code,
            Capacity = dto.Capacity,
            StoreId = dto.StoreId
        };

        await _context.Shelves.AddAsync(shelf);
        await _context.SaveChangesAsync();

        var result = await GetByIdAsync(shelf.Id);

        if (result == null)
        {
            throw new InvalidOperationException(
                "The shelf was created but could not be retrieved."
            );
        }

        return result;
    }

    public async Task<ShelfDto?> UpdateAsync(
        Guid id,
        UpdateShelfDto dto)
    {
        var shelf = await _context.Shelves
            .Where(s => s.Id == id && s.IsActive)
            .FirstOrDefaultAsync();

        if (shelf == null)
        {
            return null;
        }

        shelf.Code = dto.Code;
        shelf.Capacity = dto.Capacity;
        shelf.StoreId = dto.StoreId;

        await _context.SaveChangesAsync();

        var result = await GetByIdAsync(shelf.Id);

        return result;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var shelf = await _context.Shelves
            .Where(s => s.Id == id && s.IsActive)
            .FirstOrDefaultAsync();

        if (shelf == null)
        {
            return false;
        }

        shelf.IsActive = false;

        await _context.SaveChangesAsync();

        return true;
    }
}