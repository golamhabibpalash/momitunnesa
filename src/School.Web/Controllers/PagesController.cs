using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Web.Models;

namespace School.Web.Controllers;

public class PagesController : BaseController
{
    private readonly IPageService _pageService;

    public PagesController(IPageService pageService)
    {
        _pageService = pageService;
    }

    public async Task<IActionResult> Index(string slug)
    {
        var result = await _pageService.GetBySlugAsync(slug);

        if (!result.Success || result.Data is null)
            return NotFoundResponse("Page not found.");

        var page = result.Data;
        var viewModel = new PageViewModel
        {
            Id = page.Id,
            Title = GetLocalizedContent(page.TitleEn ?? "", page.TitleBn ?? ""),
            TitleBn = page.TitleBn,
            Slug = page.Slug,
            Content = GetLocalizedContent(page.ContentEn ?? "", page.ContentBn ?? ""),
            ContentBn = page.ContentBn,
            MetaDescription = page.MetaDescription,
            MetaKeywords = page.MetaKeywords,
            FeaturedImage = page.FeaturedImage,
            UpdatedAt = page.PublishedAt
        };

        return View(viewModel);
    }
}
