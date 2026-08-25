using MultiBranch.StockTransfer.Application.DTOs.Employee;
namespace MultiBranch.StockTransfer.Application.Interfaces;

public interface IEmployeeService
{
  Task<List<EmployeeDto>> GetAllAsync();
  Task<EmployeeDto?> GetByIdAsync(Guid id);
  Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
  Task<EmployeeDto?> UpdateAsync(Guid id,UpdateEmployeeDto dto);
  Task<bool> DeleteAsync(Guid id);

}