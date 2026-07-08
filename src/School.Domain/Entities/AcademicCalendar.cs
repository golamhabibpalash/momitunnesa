using System.ComponentModel.DataAnnotations;
using School.Domain.Common;

namespace School.Domain.Entities;

public class AcademicCalendar : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string TitleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string TitleBn { get; set; } = string.Empty;

    public string? DescriptionEn { get; set; }

    public string? DescriptionBn { get; set; }

    public DateTime EventDate { get; set; }

    public bool IsActive { get; set; } = true;
}
