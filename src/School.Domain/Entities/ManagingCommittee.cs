using System.ComponentModel.DataAnnotations;
using School.Domain.Common;

namespace School.Domain.Entities;

public class ManagingCommittee : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string NameEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string NameBn { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string DesignationEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string DesignationBn { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
