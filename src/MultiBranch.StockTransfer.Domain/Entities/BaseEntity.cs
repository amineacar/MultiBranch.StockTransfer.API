namespace MultiBranch.StockTransfer.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
    
    // Soft Delete kuralı için:
    // Hiçbir kayıt DELETE komutuyla silinmeyecek, silme işlemlerinde IsActive = false yapılacak.
    public bool IsActive { get; set; } = true;
}