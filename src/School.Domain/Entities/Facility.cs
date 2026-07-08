using System.ComponentModel.DataAnnotations;
using School.Domain.Common;
using School.Domain.Enums;

namespace School.Domain.Entities;

public class Facility : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string TitleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string TitleBn { get; set; } = string.Empty;

    public string? DescriptionEn { get; set; }

    public string? DescriptionBn { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public FacilityType FacilityType { get; set; }

    public bool IsActive { get; set; } = true;
}
