namespace MultiBranch.StockTransfer.Application.DTOs.Product;

public class ProductDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Barcode { get; set; } = string.Empty;

    public int MinimumStockLevel { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string SupplierName { get; set; } = string.Empty;
}