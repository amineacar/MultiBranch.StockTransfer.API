namespace MultiBranch.StockTransfer.Application.DTOs.Shelf;

public class CreateShelfDto
{
    public string Code { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public Guid StoreId { get; set; }
}