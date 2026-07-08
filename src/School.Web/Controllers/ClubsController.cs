using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;

namespace School.Web.Controllers;

public class ClubsController : BaseController
{
    private readonly IPageService _pageService;

    public ClubsController(IPageService pageService)
    {
        _pageService = pageService;
    }

    public async Task<IActionResult> Index()
    {
        var pageResult = await _pageService.GetBySlugAsync("clubs");
        if (pageResult.Success && pageResult.Data is not null)
        {
            ViewBag.Content = GetLocalizedContent(pageResult.Data.ContentEn ?? "", pageResult.Data.ContentBn ?? "");
            ViewBag.Title = GetLocalizedContent(pageResult.Data.TitleEn ?? "", pageResult.Data.TitleBn ?? "");
        }

        return View();
    }
}
