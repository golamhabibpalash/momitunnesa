using School.Domain.Enums;

namespace School.Application.DTOs.Notice;

public class NoticeDto
{
    public int Id { get; set; }
    public string TitleEn { get; set; } = string.Empty;
    public string TitleBn { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionBn { get; set; }
    public string NoticeCategory { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }
    public DateTime PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
