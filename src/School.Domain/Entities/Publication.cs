using System.ComponentModel.DataAnnotations;
using School.Domain.Common;
using School.Domain.Enums;

namespace School.Domain.Entities;

public class Publication : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string TitleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string TitleBn { get; set; } = string.Empty;

    public string? DescriptionEn { get; set; }

    public string? DescriptionBn { get; set; }

    [Required]
    [MaxLength(500)]
    public string FileUrl { get; set; } = string.Empty;

    public PublicationType PublicationType { get; set; }

    public DateTime PublishDate { get; set; }

    public bool IsActive { get; set; } = true;
}
