namespace MultiBranch.StockTransfer.Application.DTOs.Employee;

public class EmployeeDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string EmployeeCode { get; set; } = string.Empty;
    
    public string StoreName { get; set; } = string.Empty;
}