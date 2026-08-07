namespace MultiBranch.StockTransfer.Domain.Entities;

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;

    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

public ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();
}