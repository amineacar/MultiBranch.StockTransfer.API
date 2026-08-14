namespace MultiBranch.StockTransfer.Application.DTOs.Store;

public class StoreDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;
}