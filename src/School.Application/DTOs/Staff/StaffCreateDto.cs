using System.ComponentModel.DataAnnotations;
using School.Domain.Enums;

namespace School.Application.DTOs.Staff;

public class StaffCreateDto
{
    [Required]
    [MaxLength(200)]
    public string NameEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string NameBn { get; set; } = string.Empty;

    public Designation Designation { get; set; } = Designation.Staff;

    [MaxLength(200)]
    public string? PositionEn { get; set; }

    [MaxLength(200)]
    public string? PositionBn { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(200)]
    public string? Department { get; set; }

    public DateTime? JoiningDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}

public class StaffUpdateDto : StaffCreateDto
{
    [Required]
    public int Id { get; set; }
}
