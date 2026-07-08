using System.ComponentModel.DataAnnotations;
using School.Domain.Enums;

namespace School.Application.DTOs.News;

public class NewsCreateDto
{
    [Required]
    [MaxLength(500)]
    public string TitleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string TitleBn { get; set; } = string.Empty;

    public string? ContentEn { get; set; }
    public string? ContentBn { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public NewsCategory Category { get; set; }
    public DateTime PublishDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; }
}

public class NewsUpdateDto : NewsCreateDto
{
    [Required]
    public int Id { get; set; }
}
