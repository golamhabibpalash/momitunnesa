namespace School.Application.DTOs.Staff;

public class StaffDto
{
    public int Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameBn { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public DateTime? JoiningDate { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}
