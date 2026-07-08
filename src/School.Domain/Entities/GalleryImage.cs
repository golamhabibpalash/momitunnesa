using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using School.Domain.Common;

namespace School.Domain.Entities;

public class GalleryImage : BaseEntity
{
    public int GalleryAlbumId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    [ForeignKey(nameof(GalleryAlbumId))]
    public virtual GalleryAlbum GalleryAlbum { get; set; } = null!;
}
