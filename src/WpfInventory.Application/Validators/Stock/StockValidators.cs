using FluentValidation;
using WpfInventory.Application.Dtos.Stock;

namespace WpfInventory.Application.Validators.Stock;

public class CreateStockTransactionRequestValidator : AbstractValidator<CreateStockTransactionRequest>
{
    public CreateStockTransactionRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("请选择商品");
        RuleFor(x => x.WarehouseId).GreaterThan(0).WithMessage("请选择仓库");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("数量必须大于 0");
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0).WithMessage("单价不能小于 0");
    }
}
