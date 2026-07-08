using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace School.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    [MaxLength(200)]
    public string? FullName { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(100)]
    public string? Designation { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }
}
