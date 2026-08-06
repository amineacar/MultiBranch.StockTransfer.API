namespace MultiBranch.StockTransfer.Domain.Enums;

public enum StockMovementType
{
    StockIn = 1,       // Mağazaya veya rafa ürün girişi
    Sale = 2,          // Satış nedeniyle stok çıkışı
    Waste = 3,         // Bozulma, hasar veya çöpe atma
    RelocationOut = 4, // Ürünün mevcut raftan çıkarılması
    RelocationIn = 5,  // Ürünün yeni rafa eklenmesi
    TransferOut = 6,   // Şubeler arası transfer için kaynak raftan çıkış
    TransferIn = 7     // Transfer tamamlanınca hedef rafa giriş
}