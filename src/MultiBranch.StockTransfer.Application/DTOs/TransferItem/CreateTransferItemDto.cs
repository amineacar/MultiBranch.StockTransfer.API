namespace MultiBranch.StockTransfer.Application.DTOs.TransferItem;

public class CreateTransferItemDto
{
    public Guid ProductId { get; set; }
    public Guid SourceShelfId { get; set; }
    public Guid? TargetShelfId { get; set; }
    public int Quantity { get; set; }
}