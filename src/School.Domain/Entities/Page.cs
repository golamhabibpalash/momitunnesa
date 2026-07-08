using System.ComponentModel.DataAnnotations;
using School.Domain.Common;
using School.Domain.Enums;

namespace School.Domain.Entities;

public class Page : BaseTranslatableEntity
{
    [MaxLength(500)]
    public string? FeaturedImage { get; set; }

    public bool IsHomePage { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }

    public TemplateType TemplateType { get; set; } = TemplateType.Default;
}
