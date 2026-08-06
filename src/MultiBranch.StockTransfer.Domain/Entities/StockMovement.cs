using MultiBranch.StockTransfer.Domain.Enums;
namespace MultiBranch.StockTransfer.Domain.Entities;

public class StockMovement : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid ShelfId { get; set; }
    public Shelf Shelf { get; set; } = null!;

    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public Guid? TransferId { get; set; }
    public Transfer? Transfer { get; set; }

    public int Quantity { get; set; }
    public StockMovementType MovementType { get; set; }
    public string Reason { get; set; } = string.Empty;
}