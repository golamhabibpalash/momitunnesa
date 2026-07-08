using System.ComponentModel.DataAnnotations;
using School.Domain.Enums;

namespace School.Application.DTOs.Download;

public class DownloadCreateDto
{
    [Required]
    [MaxLength(500)]
    public string TitleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string TitleBn { get; set; } = string.Empty;

    public string? DescriptionEn { get; set; }
    public string? DescriptionBn { get; set; }

    [Required]
    [MaxLength(500)]
    public string FileUrl { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? FileType { get; set; }

    public DownloadCategory DownloadCategory { get; set; }
    public long FileSize { get; set; }
    public bool IsActive { get; set; } = true;
}

public class DownloadUpdateDto : DownloadCreateDto
{
    [Required]
    public int Id { get; set; }
}
