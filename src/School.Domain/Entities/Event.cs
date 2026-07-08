using System.ComponentModel.DataAnnotations;
using School.Domain.Common;

namespace School.Domain.Entities;

public class Event : BaseEntity
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

    public DateTime EventDate { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    [MaxLength(500)]
    public string? Location { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsFeatured { get; set; }
}
