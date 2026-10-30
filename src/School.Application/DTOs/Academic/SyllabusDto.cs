using System.ComponentModel.DataAnnotations;

namespace School.Application.DTOs.Academic;

public class SyllabusDto
{
    public int Id { get; set; }
    public string TitleEn { get; set; } = string.Empty;
    public string TitleBn { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string? FileType { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SyllabusCreateDto
{
    [Required]
    [MaxLength(500)]
    [Display(Name = "Title (English)")]
    public string TitleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [Display(Name = "Title (Bangla)")]
    public string TitleBn { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Display(Name = "Class")]
    public string ClassName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Subject { get; set; }

    /// <summary>Set by the controller from the uploaded file.</summary>
    [MaxLength(500)]
    public string FileUrl { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? FileType { get; set; }

    public bool IsActive { get; set; } = true;
}

public class SyllabusUpdateDto : SyllabusCreateDto
{
    [Required]
    public int Id { get; set; }
}
