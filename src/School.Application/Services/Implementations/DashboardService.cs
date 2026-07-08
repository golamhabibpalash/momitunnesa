using AutoMapper;
using School.Application.DTOs.Common;
using School.Application.DTOs.Dashboard;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IRepository<Teacher> _teacherRepo;
    private readonly IRepository<Staff> _staffRepo;
    private readonly IRepository<Notice> _noticeRepo;
    private readonly IRepository<News> _newsRepo;
    private readonly IRepository<GalleryImage> _galleryImageRepo;
    private readonly IRepository<Download> _downloadRepo;
    private readonly IRepository<StudentInfo> _studentRepo;
    private readonly IApplicationDataService _dataService;
    private readonly IMapper _mapper;

    public DashboardService(
        IRepository<Teacher> teacherRepo,
        IRepository<Staff> staffRepo,
        IRepository<Notice> noticeRepo,
        IRepository<News> newsRepo,
        IRepository<GalleryImage> galleryImageRepo,
        IRepository<Download> downloadRepo,
        IRepository<StudentInfo> studentRepo,
        IApplicationDataService dataService,
        IMapper mapper)
    {
        _teacherRepo = teacherRepo;
        _staffRepo = staffRepo;
        _noticeRepo = noticeRepo;
        _newsRepo = newsRepo;
        _galleryImageRepo = galleryImageRepo;
        _downloadRepo = downloadRepo;
        _studentRepo = studentRepo;
        _dataService = dataService;
        _mapper = mapper;
    }

    public async Task<ResponseDto<DashboardDto>> GetDashboardAsync()
    {
        try
        {
            var totalVisitors = await _dataService.CountVisitorsAsync();
            var totalTeachers = await _teacherRepo.CountAsync();
            var totalStaff = await _staffRepo.CountAsync();
            var totalNotices = await _noticeRepo.CountAsync();
            var totalNews = await _newsRepo.CountAsync();
            var totalGalleryImages = await _galleryImageRepo.CountAsync();
            var totalDownloads = await _downloadRepo.CountAsync();
            var totalStudents = await _studentRepo.CountAsync();
            var recentActivities = await _dataService.GetRecentActivitiesAsync(10);

            var dashboard = new DashboardDto
            {
                TotalVisitors = totalVisitors,
                TotalTeachers = totalTeachers,
                TotalStaff = totalStaff,
                TotalNotices = totalNotices,
                TotalNews = totalNews,
                TotalGalleryImages = totalGalleryImages,
                TotalDownloads = totalDownloads,
                TotalStudents = totalStudents,
                RecentActivities = recentActivities.ToList()
            };

            return ResponseDto<DashboardDto>.Ok(dashboard);
        }
        catch (Exception ex)
        {
            return ResponseDto<DashboardDto>.Fail(ex);
        }
    }
}
