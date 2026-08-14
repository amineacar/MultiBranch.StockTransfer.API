using MultiBranch.StockTransfer.Application.DTOs.TransferItem;

namespace MultiBranch.StockTransfer.Application.DTOs.Transfer;

public class CreateTransferDto
{
    public Guid SourceStoreId { get; set; }
    public Guid TargetStoreId { get; set; }
    public Guid EmployeeId { get; set; }
    public List<CreateTransferItemDto> TransferItems { get; set; } = new List<CreateTransferItemDto>();
}