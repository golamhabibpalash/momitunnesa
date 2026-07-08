using System.ComponentModel.DataAnnotations;

namespace School.Application.DTOs.Gallery;

public class GalleryVideoCreateDto
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
    [Url]
    public string YouTubeUrl { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}

public class GalleryVideoUpdateDto : GalleryVideoCreateDto
{
    [Required]
    public int Id { get; set; }
}
