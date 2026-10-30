using System.ComponentModel.DataAnnotations;

namespace School.Application.DTOs.Account;

public class ProfileDto
{
    public string? Email { get; set; }

    [MaxLength(200)]
    [Display(Name = "Full Name")]
    public string? FullName { get; set; }

    [MaxLength(100)]
    public string? Designation { get; set; }

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public string? ImageUrl { get; set; }
}

public class ChangePasswordDto
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "The new password must be at least 8 characters.")]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation do not match.")]
    [Display(Name = "Confirm New Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
