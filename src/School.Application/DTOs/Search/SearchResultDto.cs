namespace School.Application.DTOs.Search;

public class SearchResultDto
{
    public string Title { get; set; } = string.Empty;
    public string? TitleBn { get; set; }
    public string? Description { get; set; }
    public string? DescriptionBn { get; set; }
    public string? Url { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime? Date { get; set; }
}
