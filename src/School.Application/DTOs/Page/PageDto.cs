using School.Domain.Enums;

namespace School.Application.DTOs.Page;

public class PageDto
{
    public int Id { get; set; }
    public string? TitleEn { get; set; }
    public string? TitleBn { get; set; }
    public string? Slug { get; set; }
    public string? ContentEn { get; set; }
    public string? ContentBn { get; set; }
    public string? FeaturedImage { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public bool IsHomePage { get; set; }
    public bool IsPublished { get; set; }
    public bool Status { get; set; }
    public DateTime? PublishedAt { get; set; }
    public TemplateType TemplateType { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
