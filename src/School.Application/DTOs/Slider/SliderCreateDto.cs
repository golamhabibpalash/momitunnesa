using System.ComponentModel.DataAnnotations;

namespace School.Application.DTOs.Slider;

public class SliderCreateDto
{
    [MaxLength(500)]
    public string? TitleEn { get; set; }

    [MaxLength(500)]
    public string? TitleBn { get; set; }

    [MaxLength(1000)]
    public string? SubtitleEn { get; set; }

    [MaxLength(1000)]
    public string? SubtitleBn { get; set; }

    [Required]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? LinkUrl { get; set; }

    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SliderUpdateDto : SliderCreateDto
{
    [Required]
    public int Id { get; set; }
}
