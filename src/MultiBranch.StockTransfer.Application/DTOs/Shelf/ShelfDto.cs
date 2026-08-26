namespace MultiBranch.StockTransfer.Application.DTOs.Shelf;

public class ShelfDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int Capacity { get; set;}
    public string StoreName { get; set; } = string.Empty;
}