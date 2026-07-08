using System.ComponentModel.DataAnnotations;
using School.Domain.Common;
using School.Domain.Enums;

namespace School.Domain.Entities;

public class Notice : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string TitleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string TitleBn { get; set; } = string.Empty;

    public string? DescriptionEn { get; set; }

    public string? DescriptionBn { get; set; }

    public NoticeCategory NoticeCategory { get; set; }

    [MaxLength(500)]
    public string? AttachmentUrl { get; set; }

    public DateTime PublishDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsFeatured { get; set; }

    public int ViewCount { get; set; }
}
