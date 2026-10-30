using System.ComponentModel.DataAnnotations;

namespace School.Application.DTOs.Academic;

public class SubjectDto
{
    public int Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameBn { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class SubjectCreateDto
{
    [Required]
    [MaxLength(200)]
    [Display(Name = "Subject Name (English)")]
    public string NameEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Display(Name = "Subject Name (Bangla)")]
    public string NameBn { get; set; } = string.Empty;

    [MaxLength(20)]
    [Display(Name = "Subject Code")]
    public string? Code { get; set; }

    [Required]
    [MaxLength(50)]
    [Display(Name = "Class")]
    public string ClassName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

public class SubjectUpdateDto : SubjectCreateDto
{
    [Required]
    public int Id { get; set; }
}
