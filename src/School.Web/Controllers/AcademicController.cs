using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;

namespace School.Web.Controllers;

public class AcademicController : BaseController
{
    private readonly IPageService _pageService;
    private readonly IDownloadService _downloadService;

    public AcademicController(IPageService pageService, IDownloadService downloadService)
    {
        _pageService = pageService;
        _downloadService = downloadService;
    }

    public async Task<IActionResult> Calendar()
    {
        var pageResult = await _pageService.GetBySlugAsync("academic-calendar");
        if (pageResult.Success && pageResult.Data is not null)
        {
            ViewBag.Content = GetLocalizedContent(pageResult.Data.ContentEn ?? "", pageResult.Data.ContentBn ?? "");
        }

        return View();
    }

    public async Task<IActionResult> Routine()
    {
        var pageResult = await _pageService.GetBySlugAsync("class-routine");
        if (pageResult.Success && pageResult.Data is not null)
        {
            ViewBag.Content = GetLocalizedContent(pageResult.Data.ContentEn ?? "", pageResult.Data.ContentBn ?? "");
        }

        return View();
    }

    public async Task<IActionResult> Syllabus()
    {
        var pageResult = await _pageService.GetBySlugAsync("syllabus");
        if (pageResult.Success && pageResult.Data is not null)
        {
            ViewBag.Content = GetLocalizedContent(pageResult.Data.ContentEn ?? "", pageResult.Data.ContentBn ?? "");
        }

        return View();
    }
}
