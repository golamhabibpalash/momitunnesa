using System.ComponentModel.DataAnnotations;
using School.Domain.Common;
using School.Domain.Enums;

namespace School.Domain.Entities;

public class Club : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string NameEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string NameBn { get; set; } = string.Empty;

    public string? DescriptionEn { get; set; }

    public string? DescriptionBn { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public ClubType ClubType { get; set; }

    public bool IsActive { get; set; } = true;
}
