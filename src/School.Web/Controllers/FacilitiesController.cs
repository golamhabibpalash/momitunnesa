using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;

namespace School.Web.Controllers;

public class FacilitiesController : BaseController
{
    private readonly IPageService _pageService;

    public FacilitiesController(IPageService pageService)
    {
        _pageService = pageService;
    }

    public async Task<IActionResult> Index()
    {
        var pageResult = await _pageService.GetBySlugAsync("facilities");
        if (pageResult.Success && pageResult.Data is not null)
        {
            ViewBag.Content = GetLocalizedContent(pageResult.Data.ContentEn ?? "", pageResult.Data.ContentBn ?? "");
            ViewBag.Title = GetLocalizedContent(pageResult.Data.TitleEn ?? "", pageResult.Data.TitleBn ?? "");
        }

        return View();
    }
}
