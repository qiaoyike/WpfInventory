using AutoMapper;
using FluentValidation;
using WpfInventory.Application.Abstractions.Persistence;
using WpfInventory.Domain.Repositories;
using WpfInventory.Application.Abstractions.Services;
using WpfInventory.Domain.Entities;

namespace WpfInventory.Application.Services.Crud;

/// <summary>
/// 通用 CRUD 应用服务实现。
/// 把标准新增、修改、删除、查询流程集中复用，避免每个模块重复写同样代码。
/// </summary>
public class CrudAppService<TEntity, TDto, TCreateRequest, TUpdateRequest>
    : ICrudAppService<TDto, TCreateRequest, TUpdateRequest>
    where TEntity : BaseEntity
{
    private readonly IRepository<TEntity> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<TCreateRequest> _createValidator;
    private readonly IValidator<TUpdateRequest> _updateValidator;

    public CrudAppService(
        IRepository<TEntity> repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<TCreateRequest> createValidator,
        IValidator<TUpdateRequest> updateValidator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public virtual async Task<IReadOnlyList<TDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<TDto>>(entities);
    }

    public virtual async Task<TDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : _mapper.Map<TDto>(entity);
    }

    public virtual async Task<TDto> CreateAsync(TCreateRequest request, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = _mapper.Map<TEntity>(request);
        entity.CreatedAt = DateTime.UtcNow;

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TDto>(entity);
    }

    public virtual async Task UpdateAsync(TUpdateRequest request, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var id = GetId(request);
        var entity = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"未找到 Id 为 {id} 的记录");

        _mapper.Map(request, entity);
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"未找到 Id 为 {id} 的记录");

        _repository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 从更新请求中提取主键。
    /// 默认按约定取 Id 属性；特殊 DTO 可以重写。
    /// </summary>
    protected virtual int GetId(TUpdateRequest request)
    {
        return (int)(typeof(TUpdateRequest).GetProperty("Id")?.GetValue(request)
            ?? throw new InvalidOperationException("更新请求必须包含 Id 属性"));
    }
}
