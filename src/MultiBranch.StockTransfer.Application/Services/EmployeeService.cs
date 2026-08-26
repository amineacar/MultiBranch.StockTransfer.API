using Microsoft.EntityFrameworkCore;
using MultiBranch.StockTransfer.Application.DTOs.Employee;
using MultiBranch.StockTransfer.Application.Interfaces;
using MultiBranch.StockTransfer.Domain.Entities;

namespace MultiBranch.StockTransfer.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IApplicationDbContext _context;

    public EmployeeService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeDto>> GetAllAsync()
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(e => e.IsActive)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                EmployeeCode = e.EmployeeCode,
                StoreName = e.Store.Name
            })
            .ToListAsync();
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id)
{
    return await _context.Employees
        .AsNoTracking()
        .Where(e => e.Id == id && e.IsActive)
        .Select(e => new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            EmployeeCode = e.EmployeeCode,
            StoreName = e.Store.Name
        })
        .FirstOrDefaultAsync();
}

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            EmployeeCode = dto.EmployeeCode,
            StoreId = dto.StoreId
        };

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        var result = await GetByIdAsync(employee.Id);

        if (result == null)
        {
            throw new InvalidOperationException(
                "The employee was created but could not be retrieved."
            );
        }

        return result;
    }

    public async Task<EmployeeDto?> UpdateAsync(
        Guid id,
        UpdateEmployeeDto dto)
    {
        var employee = await _context.Employees
            .Where(e => e.Id == id && e.IsActive)
            .FirstOrDefaultAsync();

        if (employee == null)
        {
            return null;
        }

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;
        employee.EmployeeCode = dto.EmployeeCode;
        employee.StoreId = dto.StoreId;

        await _context.SaveChangesAsync();

        var result = await GetByIdAsync(employee.Id);

        return result;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var employee = await _context.Employees
            .Where(e => e.Id == id && e.IsActive)
            .FirstOrDefaultAsync();

        if (employee == null)
        {
            return false;
        }

        employee.IsActive = false;

        await _context.SaveChangesAsync();

        return true;
    }
}