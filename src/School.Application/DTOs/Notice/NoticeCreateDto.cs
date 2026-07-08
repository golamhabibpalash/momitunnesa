using System.ComponentModel.DataAnnotations;
using School.Domain.Enums;

namespace School.Application.DTOs.Notice;

public class NoticeCreateDto
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

    public DateTime PublishDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; }
}

public class NoticeUpdateDto : NoticeCreateDto
{
    [Required]
    public int Id { get; set; }
}
