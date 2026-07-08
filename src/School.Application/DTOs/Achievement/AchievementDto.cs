namespace School.Application.DTOs.Achievement;

public class AchievementDto
{
    public int Id { get; set; }
    public string TitleEn { get; set; } = string.Empty;
    public string TitleBn { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionBn { get; set; }
    public string? ImageUrl { get; set; }
    public string AchievementCategory { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
