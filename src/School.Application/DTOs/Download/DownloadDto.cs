namespace School.Application.DTOs.Download;

public class DownloadDto
{
    public int Id { get; set; }
    public string TitleEn { get; set; } = string.Empty;
    public string TitleBn { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionBn { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string? FileType { get; set; }
    public string DownloadCategory { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
