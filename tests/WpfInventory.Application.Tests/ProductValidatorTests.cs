using WpfInventory.Application.Dtos.Products;
using WpfInventory.Application.Validators.Products;

namespace WpfInventory.Application.Tests;

/// <summary>
/// 商品请求校验器测试。
/// </summary>
public class ProductValidatorTests
{
    private readonly CreateProductRequestValidator _createValidator = new();

    [Fact]
    public void CreateRequest_WithValidData_Passes()
    {
        var request = new CreateProductRequest
        {
            Name = "手机",
            Sku = "SKU-1001",
            UnitPrice = 1000,
            StockQuantity = 10,
            CategoryId = 1
        };

        var result = _createValidator.Validate(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void CreateRequest_WithEmptyName_Fails()
    {
        var request = new CreateProductRequest
        {
            Name = "",
            Sku = "SKU-1001",
            CategoryId = 1
        };

        var result = _createValidator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "Name");
    }

    [Fact]
    public void CreateRequest_WithInvalidCategory_Fails()
    {
        var request = new CreateProductRequest
        {
            Name = "手机",
            Sku = "SKU-1001",
            CategoryId = 0
        };

        var result = _createValidator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == "CategoryId");
    }
}