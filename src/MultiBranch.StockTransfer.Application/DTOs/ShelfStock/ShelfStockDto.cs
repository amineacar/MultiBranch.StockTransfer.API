namespace MultiBranch.StockTransfer.Application.DTOs.ShelfStock;

public class ShelfStockDto
{ 
 public Guid Id { get; set; }
 public string ShelfCode { get; set; } = string.Empty;
 public string ProductName { get; set; } = string.Empty;
 public int Quantity { get; set; }
   
}