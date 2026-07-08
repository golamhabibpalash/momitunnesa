using System.ComponentModel.DataAnnotations;
using School.Domain.Common;

namespace School.Domain.Entities;

public class StudentInfo : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string NameEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string NameBn { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Class { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Roll { get; set; }

    [MaxLength(50)]
    public string? Section { get; set; }

    [MaxLength(200)]
    public string? FatherName { get; set; }

    [MaxLength(200)]
    public string? MotherName { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;
}
