namespace MultiBranch.StockTransfer.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public int MinimumStockLevel { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public ICollection<ShelfStock> ShelfStocks { get; set; } = new List<ShelfStock>();
}