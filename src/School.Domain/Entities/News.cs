using System.ComponentModel.DataAnnotations;
using School.Domain.Common;
using School.Domain.Enums;

namespace School.Domain.Entities;

public class News : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string TitleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string TitleBn { get; set; } = string.Empty;

    public string? ContentEn { get; set; }

    public string? ContentBn { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public NewsCategory Category { get; set; }

    public DateTime PublishDate { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsFeatured { get; set; }

    public int ViewCount { get; set; }
}
