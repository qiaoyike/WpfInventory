using WpfInventory.Application.Dtos.Identity;

namespace WpfInventory.Application.Abstractions.Services;

public interface IUserManagementService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
