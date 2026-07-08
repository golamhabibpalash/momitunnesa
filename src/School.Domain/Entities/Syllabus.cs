using System.ComponentModel.DataAnnotations;
using School.Domain.Common;

namespace School.Domain.Entities;

public class Syllabus : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string TitleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string TitleBn { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string ClassName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Subject { get; set; }

    [Required]
    [MaxLength(500)]
    public string FileUrl { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? FileType { get; set; }

    public bool IsActive { get; set; } = true;
}
