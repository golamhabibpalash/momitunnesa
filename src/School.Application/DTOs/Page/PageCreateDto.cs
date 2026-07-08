using System.ComponentModel.DataAnnotations;
using School.Domain.Enums;

namespace School.Application.DTOs.Page;

public class PageCreateDto
{
    [MaxLength(500)]
    public string? TitleEn { get; set; }

    [MaxLength(500)]
    public string? TitleBn { get; set; }

    public string? ContentEn { get; set; }
    public string? ContentBn { get; set; }

    [MaxLength(500)]
    public string? FeaturedImage { get; set; }

    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [MaxLength(500)]
    public string? MetaDescription { get; set; }

    [MaxLength(300)]
    public string? MetaKeywords { get; set; }

    public bool IsHomePage { get; set; }
    public bool IsPublished { get; set; }
    public bool Status { get; set; } = true;
    public TemplateType TemplateType { get; set; } = TemplateType.Default;
    public int SortOrder { get; set; }
}

public class PageUpdateDto : PageCreateDto
{
    [Required]
    public int Id { get; set; }
}
