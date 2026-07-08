using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Web.Models;

namespace School.Web.Controllers;

public class HomeController : BaseController
{
    private readonly ISliderService _sliderService;
    private readonly INoticeService _noticeService;
    private readonly INewsService _newsService;
    private readonly IEventService _eventService;
    private readonly ITeacherService _teacherService;

    public HomeController(
        ISliderService sliderService,
        INoticeService noticeService,
        INewsService newsService,
        IEventService eventService,
        ITeacherService teacherService)
    {
        _sliderService = sliderService;
        _noticeService = noticeService;
        _newsService = newsService;
        _eventService = eventService;
        _teacherService = teacherService;
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
        ViewBag.HeadTeacher = headTeacher;

        return View();
    }
}
