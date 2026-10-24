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
    public int TotalGalleryAlbums { get; set; }
    public int VisitorsThisMonth { get; set; }
    public int ChartYear { get; set; }

    /// <summary>Visitor totals for the twelve months of <see cref="ChartYear"/>, January first.</summary>
    public int[] MonthlyVisitors { get; set; } = new int[12];

    public List<RecentActivityDto> RecentActivities { get; set; } = new();
}

public class RecentActivityDto
{
    public string? Action { get; set; }
    public string? Description { get; set; }
    public string? UserName { get; set; }
    public DateTime Timestamp { get; set; }
}
