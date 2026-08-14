namespace MultiBranch.StockTransfer.Application.DTOs.Product;

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;

    public string Barcode { get; set; } = string.Empty;

    public int MinimumStockLevel { get; set; }

    public Guid CategoryId { get; set; }

    public Guid SupplierId { get; set; }
}