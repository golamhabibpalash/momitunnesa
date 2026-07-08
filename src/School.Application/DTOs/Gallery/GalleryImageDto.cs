namespace School.Application.DTOs.Gallery;

public class GalleryImageDto
{
    public int Id { get; set; }
    public int GalleryAlbumId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GalleryImageCreateDto
{
    public int GalleryAlbumId { get; set; }

    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}
