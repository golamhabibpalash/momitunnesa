namespace School.Application.DTOs.Gallery;

public class GalleryVideoDto
{
    public int Id { get; set; }
    public string TitleEn { get; set; } = string.Empty;
    public string TitleBn { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string YouTubeUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
