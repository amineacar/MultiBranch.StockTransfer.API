namespace MultiBranch.StockTransfer.Application.DTOs.ShelfStock;

public class StockOperationResultDto
{
    public ShelfStockDto ShelfStock { get; set; } = null!;
    public List<string> Warnings { get; set; } = new();
}