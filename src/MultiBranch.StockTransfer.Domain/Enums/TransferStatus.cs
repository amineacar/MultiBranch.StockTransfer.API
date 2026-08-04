namespace MultiBranch.StockTransfer.Domain.Enums;

public enum TransferStatus
{
    Pending = 1,      // Onay bekliyor
    Approved = 2,     // Onaylandı
    Rejected = 3,     // Reddedildi
    InTransit = 4,    // Transfer yolda
    Completed = 5,    // Tamamlandı
    Cancelled = 6     // İptal edildi
}