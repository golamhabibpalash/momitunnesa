using System.ComponentModel.DataAnnotations;
using School.Domain.Common;
using School.Domain.Enums;

namespace School.Domain.Entities;

public class Achievement : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string TitleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string TitleBn { get; set; } = string.Empty;

    public string? DescriptionEn { get; set; }

    public string? DescriptionBn { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public AchievementCategory AchievementCategory { get; set; }

    public DateTime Date { get; set; }

    public bool IsActive { get; set; } = true;
}
