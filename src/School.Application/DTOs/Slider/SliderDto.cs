namespace School.Application.DTOs.Slider;

public class SliderDto
{
    public int Id { get; set; }
    public string? TitleEn { get; set; }
    public string? TitleBn { get; set; }
    public string? SubtitleEn { get; set; }
    public string? SubtitleBn { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
