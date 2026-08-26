using FluentValidation;
using MultiBranch.StockTransfer.Application.DTOs.Transfer;

namespace MultiBranch.StockTransfer.Application.Validators.Transfer;

public class CreateTransferValidator : AbstractValidator<CreateTransferDto>
{
    public CreateTransferValidator()
    {
        RuleFor(transfer => transfer.SourceStoreId)
            .NotEmpty();
        RuleFor(transfer => transfer.TargetStoreId)
            .NotEmpty()
            .NotEqual(transfer => transfer.SourceStoreId);
        RuleFor(transfer => transfer.EmployeeId)
            .NotEmpty();
        RuleFor(transfer => transfer.TransferItems)
            .NotEmpty();

        RuleForEach(transfer => transfer.TransferItems).ChildRules(item =>
        {
            item.RuleFor(transferItem => transferItem.ProductId)
            .NotEmpty();
            item.RuleFor(transferItem => transferItem.SourceShelfId)
            .NotEmpty();
            item.RuleFor(transferItem => transferItem.TargetShelfId)
            .NotEmpty();
            item.RuleFor(transferItem => transferItem.Quantity)
            .GreaterThan(0);
        });
    }
}
