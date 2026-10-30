using System.ComponentModel.DataAnnotations;

namespace School.Application.DTOs.Academic;

public class RoutineDto
{
    public int Id { get; set; }
    public string ClassNameEn { get; set; } = string.Empty;
    public string ClassNameBn { get; set; } = string.Empty;
    public string? Section { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public string SubjectEn { get; set; } = string.Empty;
    public string SubjectBn { get; set; } = string.Empty;
    public string? TeacherEn { get; set; }
    public string? TeacherBn { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Room { get; set; }
    public bool IsActive { get; set; }
}

public class RoutineCreateDto
{
    [Required]
    [MaxLength(100)]
    [Display(Name = "Class (English)")]
    public string ClassNameEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Display(Name = "Class (Bangla)")]
    public string ClassNameBn { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Section { get; set; }

    [Required]
    [MaxLength(20)]
    [Display(Name = "Day")]
    public string DayOfWeek { get; set; } = "Sunday";

    [Required]
    [MaxLength(200)]
    [Display(Name = "Subject (English)")]
    public string SubjectEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Display(Name = "Subject (Bangla)")]
    public string SubjectBn { get; set; } = string.Empty;

    [MaxLength(200)]
    [Display(Name = "Teacher (English)")]
    public string? TeacherEn { get; set; }

    [MaxLength(200)]
    [Display(Name = "Teacher (Bangla)")]
    public string? TeacherBn { get; set; }

    [Display(Name = "Start Time")]
    public TimeSpan StartTime { get; set; } = new(10, 0, 0);

    [Display(Name = "End Time")]
    public TimeSpan EndTime { get; set; } = new(10, 45, 0);

    [MaxLength(50)]
    public string? Room { get; set; }

    public bool IsActive { get; set; } = true;
}

public class RoutineUpdateDto : RoutineCreateDto
{
    [Required]
    public int Id { get; set; }
}
