namespace School.Application.DTOs.News;

public class NewsDto
{
    public int Id { get; set; }
    public string TitleEn { get; set; } = string.Empty;
    public string TitleBn { get; set; } = string.Empty;
    public string? ContentEn { get; set; }
    public string? ContentBn { get; set; }
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
