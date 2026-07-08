using System.ComponentModel.DataAnnotations;

namespace School.Application.DTOs.Gallery;

public class GalleryAlbumCreateDto
{
    [Required]
    [MaxLength(500)]
    public string TitleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string TitleBn { get; set; } = string.Empty;

    public string? DescriptionEn { get; set; }
    public string? DescriptionBn { get; set; }

    [MaxLength(500)]
    public string? CoverImageUrl { get; set; }

    public bool IsActive { get; set; } = true;
}

public class GalleryAlbumUpdateDto : GalleryAlbumCreateDto
{
    [Required]
    public int Id { get; set; }
}
