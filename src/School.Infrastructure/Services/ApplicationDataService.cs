using System.Text.Json;
using System.Text.Json.Serialization;
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

    public async Task<int[]> GetMonthlyVisitorsAsync(int year)
    {
        var totals = await _context.VisitorCounters
            .Where(v => v.Date.Year == year)
            .GroupBy(v => v.Date.Month)
            .Select(g => new { Month = g.Key, Total = g.Sum(v => v.Count) })
            .ToListAsync();

        var months = new int[12];
        foreach (var t in totals)
            months[t.Month - 1] = t.Total;

        return months;
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

    public async Task LogActivityAsync(ActivityLog log)
    {
        _context.ActivityLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<(List<ActivityLog> Items, int TotalCount)> GetActivityLogsAsync(int page, int pageSize, string? search)
    {
        var query = _context.ActivityLogs.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(a =>
                (a.UserName != null && a.UserName.Contains(term)) ||
                (a.Action != null && a.Action.Contains(term)) ||
                (a.Controller != null && a.Controller.Contains(term)) ||
                (a.Description != null && a.Description.Contains(term)));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((Math.Max(page, 1) - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }

    public async Task<int> DeleteActivityLogsOlderThanAsync(DateTime cutoffUtc)
    {
        return await _context.ActivityLogs.Where(a => a.Timestamp < cutoffUtc).ExecuteDeleteAsync();
    }

    public async Task ExportContentAsJsonAsync(Stream output)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        await using var writer = new Utf8JsonWriter(output, new JsonWriterOptions { Indented = true, Encoder = options.Encoder });
        writer.WriteStartObject();
        writer.WriteString("exportedAtUtc", DateTime.UtcNow);

        async Task WriteAsync<T>(string name, IQueryable<T> set) where T : class
        {
            writer.WritePropertyName(name);
            JsonSerializer.Serialize(writer, await set.AsNoTracking().ToListAsync(), options);
        }

        await WriteAsync("SchoolInfo", _context.SchoolInfos);
        await WriteAsync("SiteSettings", _context.SiteSettings);
        await WriteAsync("SocialLinks", _context.SocialLinks);
        await WriteAsync("Menus", _context.Menus);
        await WriteAsync("Pages", _context.Pages);
        await WriteAsync("Sliders", _context.Sliders);
        await WriteAsync("Notices", _context.Notices);
        await WriteAsync("News", _context.News);
        await WriteAsync("Events", _context.Events);
        await WriteAsync("GalleryAlbums", _context.GalleryAlbums);
        await WriteAsync("GalleryImages", _context.GalleryImages);
        await WriteAsync("GalleryVideos", _context.GalleryVideos);
        await WriteAsync("Teachers", _context.Teachers);
        await WriteAsync("Staffs", _context.Staffs);
        await WriteAsync("ManagingCommittees", _context.ManagingCommittees);
        await WriteAsync("Downloads", _context.Downloads);
        await WriteAsync("Achievements", _context.Achievements);
        await WriteAsync("Facilities", _context.Facilities);
        await WriteAsync("Clubs", _context.Clubs);
        await WriteAsync("AcademicCalendars", _context.AcademicCalendars);
        await WriteAsync("Routines", _context.Routines);
        await WriteAsync("Subjects", _context.Subjects);
        await WriteAsync("Syllabuses", _context.Syllabuses);
        await WriteAsync("StudentInfos", _context.StudentInfos);
        await WriteAsync("Publications", _context.Publications);
        await WriteAsync("ImportantLinks", _context.ImportantLinks);
        await WriteAsync("Admissions", _context.Admissions);
        await WriteAsync("ContactMessages", _context.ContactMessages);
        await WriteAsync("VisitorCounters", _context.VisitorCounters);

        writer.WriteEndObject();
        await writer.FlushAsync();
    }
}
