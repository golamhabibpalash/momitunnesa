using System.ComponentModel.DataAnnotations;
using School.Domain.Common;

namespace School.Domain.Entities;

public class Subject : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string NameEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string NameBn { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Code { get; set; }

    [Required]
    [MaxLength(50)]
    public string ClassName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
