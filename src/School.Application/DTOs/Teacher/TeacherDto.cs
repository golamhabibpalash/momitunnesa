using School.Domain.Enums;

namespace School.Application.DTOs.Teacher;

public class TeacherDto
{
    public int Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameBn { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? QualificationEn { get; set; }
    public string? QualificationBn { get; set; }
    public string? SubjectEn { get; set; }
    public string? SubjectBn { get; set; }
    public DateTime? JoiningDate { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public bool IsHeadTeacher { get; set; }
    public bool IsAssistantHeadTeacher { get; set; }
}
