using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Domain.Enums;
using School.Web.Models;

namespace School.Web.Controllers;

public class HomeController : BaseController
{
    private readonly ISliderService _sliderService;
    private readonly INoticeService _noticeService;
    private readonly INewsService _newsService;
    private readonly IEventService _eventService;
    private readonly ITeacherService _teacherService;
    private readonly IStaffService _staffService;
    private readonly IAchievementService _achievementService;
    private readonly ISettingsService _settingsService;

    public HomeController(
        ISliderService sliderService,
        INoticeService noticeService,
        INewsService newsService,
        IEventService eventService,
        ITeacherService teacherService,
        IStaffService staffService,
        IAchievementService achievementService,
        ISettingsService settingsService)
    {
        _staffService = staffService;
        _achievementService = achievementService;
        _settingsService = settingsService;
        _sliderService = sliderService;
        _noticeService = noticeService;
        _newsService = newsService;
        _eventService = eventService;
        _teacherService = teacherService;
    }

    public IActionResult SwitchLanguage(string lang, string returnUrl = "/")
    {
        var culture = lang switch
        {
            "bn" => "bn",
            _ => "en"
        };

        HttpContext.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(returnUrl);
    }

    public async Task<IActionResult> Index()
    {
        var slidersResult = await _sliderService.GetActiveSlidersAsync();
        var noticesResult = await _noticeService.GetFeaturedNoticesAsync();
        var newsResult = await _newsService.GetFeaturedNewsAsync();
        var eventsResult = await _eventService.GetUpcomingEventsAsync();
        var headTeacherResult = await _teacherService.GetHeadTeacherAsync();

        var sliders = slidersResult.Success ? slidersResult.Data : null;
        var notices = noticesResult.Success ? noticesResult.Data : null;
        var news = newsResult.Success ? newsResult.Data : null;
        var events = eventsResult.Success ? eventsResult.Data : null;
        var headTeacher = headTeacherResult.Success ? headTeacherResult.Data : null;

        ViewBag.Sliders = sliders;
        ViewBag.Notices = notices?.Take(5);
        ViewBag.News = news?.Take(3);
        ViewBag.Events = events?.Take(4);

        // Homepage counters come from the same data as the Teachers, Staff and Achievements pages.
        var teachersResult = await _teacherService.GetActiveTeachersAsync();
        var staffResult = await _staffService.GetActiveStaffAsync();
        var achievementsResult = await _achievementService.GetAllAsync();
        var studentsSetting = await _settingsService.GetSettingByKeyAsync("TotalStudents");
        var activeAchievements = achievementsResult.Data?.Where(a => a.IsActive).ToList() ?? new();

        ViewBag.AchievementCounts = Enum.GetValues<AchievementCategory>()
            .ToDictionary(c => c, c => activeAchievements.Count(a => a.AchievementCategory == c.ToString()));
        ViewBag.Statistics = new StatisticsViewModel
        {
            Teachers = teachersResult.Data?.Count() ?? 0,
            Staff = staffResult.Data?.Count() ?? 0,
            Achievements = activeAchievements.Count,
            Students = int.TryParse(studentsSetting.Data?.Value, out var students) ? students : 0
        };

        if (headTeacher is not null)
        {
            ViewBag.HeadTeacher = new HeadTeacherMessageViewModel
            {
                Name = headTeacher.NameEn,
                NameBn = headTeacher.NameBn,
                ImageUrl = headTeacher.ImageUrl
            };
        }

        return View();
    }
}
