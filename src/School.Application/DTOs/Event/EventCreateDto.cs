using System.ComponentModel.DataAnnotations;

namespace School.Application.DTOs.Event;

public class EventCreateDto
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
    public string? ImageUrl { get; set; }

    public DateTime EventDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }

    [MaxLength(500)]
    public string? Location { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; }
}

public class EventUpdateDto : EventCreateDto
{
    [Required]
    public int Id { get; set; }
}
