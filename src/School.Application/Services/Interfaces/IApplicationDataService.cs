using School.Application.DTOs.Dashboard;
using School.Domain.Entities;

namespace School.Application.Services.Interfaces;

public interface IApplicationDataService
{
    Task<int> CountVisitorsAsync();

    /// <summary>Visitor totals per month for the given year, as a twelve-slot array starting at January.</summary>
    Task<int[]> GetMonthlyVisitorsAsync(int year);

    Task<IEnumerable<RecentActivityDto>> GetRecentActivitiesAsync(int count);
    Task<SchoolInfo?> GetSchoolInfoAsync();
    Task AddOrUpdateSchoolInfoAsync(SchoolInfo schoolInfo);
    Task<int> SaveChangesAsync();

    Task LogActivityAsync(ActivityLog log);

    /// <summary>Newest first; <paramref name="search"/> matches user, action, section or description.</summary>
    Task<(List<ActivityLog> Items, int TotalCount)> GetActivityLogsAsync(int page, int pageSize, string? search);

    Task<int> DeleteActivityLogsOlderThanAsync(DateTime cutoffUtc);

    /// <summary>Writes every content table (not user accounts or passwords) as one JSON document.</summary>
    Task ExportContentAsJsonAsync(Stream output);
}
