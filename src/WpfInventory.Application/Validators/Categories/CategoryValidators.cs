using FluentValidation;
using WpfInventory.Application.Dtos.Categories;

namespace WpfInventory.Application.Validators.Categories;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("分类名称不能为空").MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().WithMessage("分类名称不能为空").MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
