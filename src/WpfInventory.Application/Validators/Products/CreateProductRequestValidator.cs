using FluentValidation;
using WpfInventory.Application.Dtos.Products;

namespace WpfInventory.Application.Validators.Products;

/// <summary>
/// 创建商品请求的校验器。
/// 校验规则集中在 Application 层，界面或其他入口可以复用。
/// </summary>
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("商品名称不能为空")
            .MaximumLength(200).WithMessage("商品名称不能超过 200 个字符");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("商品编码不能为空")
            .MaximumLength(50).WithMessage("商品编码不能超过 50 个字符");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("单价不能小于 0");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("库存数量不能小于 0");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("请选择有效的商品分类");
    }
}
