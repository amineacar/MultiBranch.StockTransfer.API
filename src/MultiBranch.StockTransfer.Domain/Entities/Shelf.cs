namespace MultiBranch.StockTransfer.Domain.Entities;

public class Shelf : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public int Capacity { get; set; }

    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;

    public ICollection<ShelfStock> ShelfStocks { get; set; } = new List<ShelfStock>();
}