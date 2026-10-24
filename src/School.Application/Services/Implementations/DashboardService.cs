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
    private readonly IRepository<GalleryAlbum> _galleryAlbumRepo;
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
        IRepository<GalleryAlbum> galleryAlbumRepo,
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
        _galleryAlbumRepo = galleryAlbumRepo;
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
            var totalGalleryAlbums = await _galleryAlbumRepo.CountAsync();
            var recentActivities = await _dataService.GetRecentActivitiesAsync(10);

            var chartYear = DateTime.Now.Year;
            var monthlyVisitors = await _dataService.GetMonthlyVisitorsAsync(chartYear);

            var dashboard = new DashboardDto
            {
                TotalVisitors = totalVisitors,
                TotalTeachers = totalTeachers,
                TotalStaff = totalStaff,
                TotalNotices = totalNotices,
                TotalNews = totalNews,
                TotalGalleryImages = totalGalleryImages,
                TotalGalleryAlbums = totalGalleryAlbums,
                TotalDownloads = totalDownloads,
                TotalStudents = totalStudents,
                ChartYear = chartYear,
                MonthlyVisitors = monthlyVisitors,
                VisitorsThisMonth = monthlyVisitors[DateTime.Now.Month - 1],
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
