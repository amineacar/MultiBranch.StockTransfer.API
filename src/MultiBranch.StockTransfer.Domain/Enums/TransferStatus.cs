namespace MultiBranch.StockTransfer.Domain.Enums;

public enum TransferStatus
{
  
    InTransit = 4,    // Transfer yolda
    Completed = 5,    // Tamamlandı
    Cancelled = 6     // İptal edildi
}