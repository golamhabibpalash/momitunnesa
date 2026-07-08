using Microsoft.EntityFrameworkCore;
using School.Application.DTOs.Dashboard;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Infrastructure.Data;

namespace School.Infrastructure.Services;

public class ApplicationDataService : IApplicationDataService
{
    private readonly ApplicationDbContext _context;

    public ApplicationDataService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CountVisitorsAsync()
    {
        return await _context.VisitorCounters.SumAsync(v => v.Count);
    }

    public async Task<IEnumerable<RecentActivityDto>> GetRecentActivitiesAsync(int count)
    {
        return await _context.ActivityLogs
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .Select(a => new RecentActivityDto
            {
                Action = a.Action,
                Description = a.Description,
                UserName = a.UserName,
                Timestamp = a.Timestamp
            })
            .ToListAsync();
    }

    public async Task<SchoolInfo?> GetSchoolInfoAsync()
    {
        return await _context.SchoolInfos.FirstOrDefaultAsync();
    }

    public async Task AddOrUpdateSchoolInfoAsync(SchoolInfo schoolInfo)
    {
        var existing = await _context.SchoolInfos.FirstOrDefaultAsync();
        if (existing is null)
        {
            await _context.SchoolInfos.AddAsync(schoolInfo);
        }
        else
        {
            existing.SchoolNameEn = schoolInfo.SchoolNameEn;
            existing.SchoolNameBn = schoolInfo.SchoolNameBn;
            existing.SloganEn = schoolInfo.SloganEn;
            existing.SloganBn = schoolInfo.SloganBn;
            existing.AddressEn = schoolInfo.AddressEn;
            existing.AddressBn = schoolInfo.AddressBn;
            existing.Phone = schoolInfo.Phone;
            existing.Email = schoolInfo.Email;
            existing.Website = schoolInfo.Website;
            existing.LogoUrl = schoolInfo.LogoUrl;
            existing.FaviconUrl = schoolInfo.FaviconUrl;
            existing.GoogleMapEmbedUrl = schoolInfo.GoogleMapEmbedUrl;
            existing.FooterTextEn = schoolInfo.FooterTextEn;
            existing.FooterTextBn = schoolInfo.FooterTextBn;
            existing.AboutShortEn = schoolInfo.AboutShortEn;
            existing.AboutShortBn = schoolInfo.AboutShortBn;
            existing.MissionEn = schoolInfo.MissionEn;
            existing.MissionBn = schoolInfo.MissionBn;
            existing.VisionEn = schoolInfo.VisionEn;
            existing.VisionBn = schoolInfo.VisionBn;
            existing.HistoryEn = schoolInfo.HistoryEn;
            existing.HistoryBn = schoolInfo.HistoryBn;
            existing.ObjectiveEn = schoolInfo.ObjectiveEn;
            existing.ObjectiveBn = schoolInfo.ObjectiveBn;
            existing.FuturePlanEn = schoolInfo.FuturePlanEn;
            existing.FuturePlanBn = schoolInfo.FuturePlanBn;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
