using System.ComponentModel.DataAnnotations;
using School.Domain.Enums;

namespace School.Application.DTOs.Teacher;

public class TeacherCreateDto
{
    [Required]
    [MaxLength(200)]
    public string NameEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string NameBn { get; set; } = string.Empty;

    public Designation Designation { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? QualificationEn { get; set; }

    [MaxLength(500)]
    public string? QualificationBn { get; set; }

    [MaxLength(200)]
    public string? SubjectEn { get; set; }

    [MaxLength(200)]
    public string? SubjectBn { get; set; }

    public DateTime? JoiningDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
    public bool IsHeadTeacher { get; set; }
    public bool IsAssistantHeadTeacher { get; set; }
}

public class TeacherUpdateDto : TeacherCreateDto
{
    [Required]
    public int Id { get; set; }
}
