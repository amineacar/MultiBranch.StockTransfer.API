namespace MultiBranch.StockTransfer.Domain.Entities;

using MultiBranch.StockTransfer.Domain.Enums;

public class Transfer : BaseEntity
{
    public Guid SourceStoreId { get; set; }
    public Store SourceStore { get; set; } = null!;

    public Guid TargetStoreId { get; set; }
    public Store TargetStore { get; set; } = null!;

    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public TransferStatus Status { get; set; } = TransferStatus.InTransit;

    public ICollection<TransferItem> TransferItems { get; set; } = new List<TransferItem>();
}