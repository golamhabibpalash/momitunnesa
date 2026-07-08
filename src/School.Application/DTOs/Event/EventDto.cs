namespace School.Application.DTOs.Event;

public class EventDto
{
    public int Id { get; set; }
    public string TitleEn { get; set; } = string.Empty;
    public string TitleBn { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionBn { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime EventDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }
}
