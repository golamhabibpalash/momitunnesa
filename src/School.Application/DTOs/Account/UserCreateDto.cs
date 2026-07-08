using System.ComponentModel.DataAnnotations;

namespace School.Application.DTOs.Account;

public class UserCreateDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;

    public List<string> Roles { get; set; } = new();
}

public class UserUpdateDto
{
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;

    public List<string> Roles { get; set; } = new();
}
