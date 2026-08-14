using MultiBranch.StockTransfer.Application.DTOs.TransferItem;
using MultiBranch.StockTransfer.Domain.Enums;

namespace MultiBranch.StockTransfer.Application.DTOs.Transfer;

public class TransferDto
{
    public Guid Id { get; set; }
    public Guid SourceStoreId { get; set; }
    public Guid TargetStoreId { get; set; }
    public Guid EmployeeId { get; set; }
    public TransferStatus Status { get; set; }
    public List<TransferItemDto> TransferItems { get; set; } = new();
     public List<string> Warnings { get; set; } = new();
}