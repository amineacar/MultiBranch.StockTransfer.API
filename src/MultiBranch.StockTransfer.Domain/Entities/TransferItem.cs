namespace MultiBranch.StockTransfer.Domain.Entities;

public class TransferItem : BaseEntity
{
    public Guid TransferId { get; set; }
    public Transfer Transfer { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid SourceShelfId { get; set; }
    public Shelf SourceShelf { get; set; } = null!;
    public Guid? TargetShelfId { get; set; }
    public Shelf? TargetShelf { get; set; }
    public int Quantity { get; set; }
}