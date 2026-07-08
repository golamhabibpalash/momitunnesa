using System.ComponentModel.DataAnnotations;
using School.Domain.Common;

namespace School.Domain.Entities;

public class SiteSetting : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Key { get; set; } = string.Empty;

    public string? Value { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
