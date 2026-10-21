namespace WpfInventory.Application.Abstractions.Persistence;

/// <summary>
/// 工作单元接口。
/// 它负责统一提交多个仓储操作，并支持事务。
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// 保存所有已跟踪的变更。
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 在一个数据库事务中执行指定操作。
    /// 如果 action 抛异常，事务会回滚。
    /// </summary>
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);
}
