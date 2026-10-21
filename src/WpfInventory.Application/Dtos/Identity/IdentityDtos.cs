namespace WpfInventory.Application.Dtos.Identity;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public IReadOnlyList<string> RoleCodes { get; set; } = new List<string>();
    public IReadOnlyList<string> PermissionCodes { get; set; } = new List<string>();
}

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public IReadOnlyList<int> RoleIds { get; set; } = new List<int>();
}

public class UpdateUserRequest
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public IReadOnlyList<int> RoleIds { get; set; } = new List<int>();
}

public class RoleDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IReadOnlyList<int> PermissionIds { get; set; } = new List<int>();
}

public class CreateRoleRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IReadOnlyList<int> PermissionIds { get; set; } = new List<int>();
}

public class UpdateRoleRequest
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IReadOnlyList<int> PermissionIds { get; set; } = new List<int>();
}

public class PermissionDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
