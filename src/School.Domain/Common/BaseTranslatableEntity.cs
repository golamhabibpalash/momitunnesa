using System.ComponentModel.DataAnnotations;

namespace School.Domain.Common;

public abstract class BaseTranslatableEntity : BaseEntity
{
    [MaxLength(500)]
    public string? TitleBn { get; set; }

    [MaxLength(500)]
    public string? TitleEn { get; set; }

    public string? Slug { get; set; }

    public string? ContentBn { get; set; }

    public string? ContentEn { get; set; }

    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [MaxLength(500)]
    public string? MetaDescription { get; set; }

    [MaxLength(300)]
    public string? MetaKeywords { get; set; }

    public bool Status { get; set; } = true;

    public int SortOrder { get; set; }
}
