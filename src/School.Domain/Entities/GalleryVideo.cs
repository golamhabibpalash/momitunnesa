using System.ComponentModel.DataAnnotations;
using School.Domain.Common;
using School.Domain.Enums;

namespace School.Domain.Entities;

public class GalleryVideo : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string TitleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string TitleBn { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [MaxLength(500)]
    public string YouTubeUrl { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }
}
