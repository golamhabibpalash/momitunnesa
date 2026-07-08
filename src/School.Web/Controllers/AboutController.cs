using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Web.Models;

namespace School.Web.Controllers;

public class AboutController : BaseController
{
    private readonly IPageService _pageService;
    private readonly ISettingsService _settingsService;
    private readonly ITeacherService _teacherService;

    public AboutController(
        IPageService pageService,
        ISettingsService settingsService,
        ITeacherService teacherService)
    {
        _pageService = pageService;
        _settingsService = settingsService;
        _teacherService = teacherService;
    }

    public async Task<IActionResult> Index()
    {
        var homePageResult = await _pageService.GetHomePageAsync();
        var schoolInfoResult = await _settingsService.GetSchoolInfoAsync();
        var headTeacherResult = await _teacherService.GetHeadTeacherAsync();

        if (schoolInfoResult.Success && schoolInfoResult.Data is not null)
        {
            var info = schoolInfoResult.Data;
            ViewBag.SchoolName = GetLocalizedContent(info.SchoolNameEn, info.SchoolNameBn);
            ViewBag.Mission = GetLocalizedContent(info.MissionEn ?? "", info.MissionBn ?? "");
            ViewBag.Vision = GetLocalizedContent(info.VisionEn ?? "", info.VisionBn ?? "");
            ViewBag.History = GetLocalizedContent(info.HistoryEn ?? "", info.HistoryBn ?? "");
            ViewBag.Objective = GetLocalizedContent(info.ObjectiveEn ?? "", info.ObjectiveBn ?? "");
            ViewBag.FuturePlan = GetLocalizedContent(info.FuturePlanEn ?? "", info.FuturePlanBn ?? "");
            ViewBag.AboutShort = GetLocalizedContent(info.AboutShortEn ?? "", info.AboutShortBn ?? "");
        }

        if (homePageResult.Success && homePageResult.Data is not null)
        {
            var page = homePageResult.Data;
            ViewBag.PageContent = GetLocalizedContent(page.ContentEn ?? "", page.ContentBn ?? "");
            ViewBag.PageTitle = GetLocalizedContent(page.TitleEn ?? "", page.TitleBn ?? "");
        }

        if (headTeacherResult.Success && headTeacherResult.Data is not null)
        {
            var ht = headTeacherResult.Data;
            ViewBag.HeadTeacher = new HeadTeacherMessageViewModel
            {
                Name = GetLocalizedContent(ht.NameEn, ht.NameBn),
                NameBn = ht.NameBn,
                ImageUrl = ht.ImageUrl
            };
        }

        return View();
    }

    public async Task<IActionResult> SchoolProfile()
    {
        var schoolInfoResult = await _settingsService.GetSchoolInfoAsync();
        var teacherResult = await _teacherService.GetActiveTeachersAsync();

        if (!schoolInfoResult.Success || schoolInfoResult.Data is null)
            return NotFoundResponse("School information not found.");

        var info = schoolInfoResult.Data;

        var viewModel = new SchoolProfileViewModel
        {
            Eiin = null,
            SchoolCode = null,
            EstablishedYear = null,
            Board = null,
            TeacherCount = teacherResult.Success && teacherResult.Data is not null ? teacherResult.Data.Count() : 0,
            StudentCount = 0,
            Division = null,
            District = null,
            Upazila = null,
            PostOffice = null,
            PostCode = null,
        };

        return View(viewModel);
    }
}
