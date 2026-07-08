namespace School.Application.DTOs.Account;

public class RoleDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class RoleCreateDto
{
    public string Name { get; set; } = string.Empty;
}
