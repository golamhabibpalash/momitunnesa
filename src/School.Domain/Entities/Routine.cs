using System.ComponentModel.DataAnnotations;
using School.Domain.Common;

namespace School.Domain.Entities;

public class Routine : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string ClassNameEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ClassNameBn { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Section { get; set; }

    [Required]
    [MaxLength(20)]
    public string DayOfWeek { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string SubjectEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string SubjectBn { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? TeacherEn { get; set; }

    [MaxLength(200)]
    public string? TeacherBn { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    [MaxLength(50)]
    public string? Room { get; set; }

    public bool IsActive { get; set; } = true;
}
