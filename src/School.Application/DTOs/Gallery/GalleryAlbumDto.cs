namespace School.Application.DTOs.Gallery;

public class GalleryAlbumDto
{
    public int Id { get; set; }
    public string TitleEn { get; set; } = string.Empty;
    public string TitleBn { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionBn { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsActive { get; set; }
    public int ImageCount { get; set; }
    public List<GalleryImageDto> GalleryImages { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
