using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WpfInventory.Application.Dtos.Categories;
using WpfInventory.Application.Mapping;
using WpfInventory.Application.Services.Crud;
using WpfInventory.Application.Validators.Categories;
using WpfInventory.Domain.Entities;
using WpfInventory.Infrastructure.Data;
using WpfInventory.Infrastructure.Persistence;
using WpfInventory.Infrastructure.Persistence.Repositories;

namespace WpfInventory.Infrastructure.Tests;

/// <summary>
/// 分类 CRUD 应用服务集成测试。
/// 复现“新增分类”的完整服务链路。
/// </summary>
public class CategoryCrudServiceTests
{
    [Fact]
    public async Task CreateAsync_PersistsCategory()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using var dbContext = new AppDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<CategoryMappingProfile>()).CreateMapper();
        var service = new CrudAppService<Category, CategoryDto, CreateCategoryRequest, UpdateCategoryRequest>(
            new Repository<Category>(dbContext),
            new EfUnitOfWork(dbContext),
            mapper,
            new CreateCategoryRequestValidator(),
            new UpdateCategoryRequestValidator());

        var dto = await service.CreateAsync(new CreateCategoryRequest
        {
            Name = "电子",
            Description = "电子类商品"
        });

        Assert.True(dto.Id > 0);
        Assert.Equal(1, await dbContext.Categories.CountAsync());
    }
}