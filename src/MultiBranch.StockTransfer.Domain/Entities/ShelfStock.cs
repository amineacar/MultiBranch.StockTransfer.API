namespace MultiBranch.StockTransfer.Domain.Entities;

public class ShelfStock : BaseEntity
{
    public Guid ShelfId { get; set; }
    public Shelf Shelf { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
}