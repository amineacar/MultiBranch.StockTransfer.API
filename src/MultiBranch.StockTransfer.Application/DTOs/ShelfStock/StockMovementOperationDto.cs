using MultiBranch.StockTransfer.Domain.Enums;

namespace MultiBranch.StockTransfer.Application.DTOs.ShelfStock;

public class StockMovementOperationDto
{
    public Guid ShelfId { get; set; }

    public Guid ProductId { get; set; }

    public Guid EmployeeId { get; set; }

    public int Quantity { get; set; }

    public StockMovementType MovementType { get; set; }

    public string Reason { get; set; } = string.Empty;
}