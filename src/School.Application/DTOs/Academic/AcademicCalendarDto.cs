using System.ComponentModel.DataAnnotations;

namespace School.Application.DTOs.Academic;

public class AcademicCalendarDto
{
    public int Id { get; set; }
    public string TitleEn { get; set; } = string.Empty;
    public string TitleBn { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionBn { get; set; }
    public DateTime EventDate { get; set; }
    public bool IsActive { get; set; }
}

public class AcademicCalendarCreateDto
{
    [Required]
    [MaxLength(500)]
    [Display(Name = "Title (English)")]
    public string TitleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [Display(Name = "Title (Bangla)")]
    public string TitleBn { get; set; } = string.Empty;

    [Display(Name = "Description (English)")]
    public string? DescriptionEn { get; set; }

    [Display(Name = "Description (Bangla)")]
    public string? DescriptionBn { get; set; }

    [Display(Name = "Date")]
    public DateTime EventDate { get; set; } = DateTime.Today;

    public bool IsActive { get; set; } = true;
}

public class AcademicCalendarUpdateDto : AcademicCalendarCreateDto
{
    [Required]
    public int Id { get; set; }
}
