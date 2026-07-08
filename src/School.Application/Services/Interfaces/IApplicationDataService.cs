using School.Application.DTOs.Dashboard;
using School.Domain.Entities;

namespace School.Application.Services.Interfaces;

public interface IApplicationDataService
{
    Task<int> CountVisitorsAsync();
    Task<IEnumerable<RecentActivityDto>> GetRecentActivitiesAsync(int count);
    Task<SchoolInfo?> GetSchoolInfoAsync();
    Task AddOrUpdateSchoolInfoAsync(SchoolInfo schoolInfo);
    Task<int> SaveChangesAsync();
}
