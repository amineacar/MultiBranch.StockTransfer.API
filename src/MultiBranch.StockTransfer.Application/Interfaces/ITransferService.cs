using MultiBranch.StockTransfer.Application.DTOs.Transfer;

namespace MultiBranch.StockTransfer.Application.Interfaces;

public interface ITransferService
{
    Task<List<TransferDto>> GetAllAsync();

    Task<TransferDto?> GetByIdAsync(Guid id);

    Task<TransferDto> CreateAsync(CreateTransferDto dto);
    Task<TransferDto?> CompleteAsync(Guid id ,Guid employeeId);
    Task<TransferDto?> CancelAsync(Guid id, Guid employeeId);

    
}