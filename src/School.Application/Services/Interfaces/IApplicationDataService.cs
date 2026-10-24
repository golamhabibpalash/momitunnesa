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
}
