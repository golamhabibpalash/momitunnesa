namespace School.Application.DTOs.Dashboard;

public class DashboardDto
{
    public int TotalVisitors { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalStaff { get; set; }
    public int TotalNotices { get; set; }
    public int TotalNews { get; set; }
    public int TotalGalleryImages { get; set; }
    public int TotalDownloads { get; set; }
    public int TotalStudents { get; set; }
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
}

public class RecentActivityDto
{
    public string? Action { get; set; }
    public string? Description { get; set; }
    public string? UserName { get; set; }
    public DateTime Timestamp { get; set; }
}
