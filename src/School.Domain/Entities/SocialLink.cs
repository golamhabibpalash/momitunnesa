using System.ComponentModel.DataAnnotations;
using School.Domain.Common;

namespace School.Domain.Entities;

public class SocialLink : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Platform { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Icon { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
