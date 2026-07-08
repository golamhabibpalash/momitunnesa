using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.News;
using School.Application.Services.Interfaces;
using School.Domain.Enums;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class NewsController : BaseAdminController
{
    private readonly INewsService _newsService;

    public NewsController(
        INewsService newsService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _newsService = newsService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _newsService.GetAllAsync();
        const int pageSize = 20;

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load news.");
            return View(new List<NewsDto>());
        }

        var newsItems = result.Data.OrderByDescending(n => n.PublishDate).ToList();
        var totalPages = (int)Math.Ceiling(newsItems.Count / (double)pageSize);
        var pagedItems = newsItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(pagedItems);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new NewsCreateDto { PublishDate = DateTime.Now });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NewsCreateDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _newsService.CreateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create news.");
            return View(model);
        }

        SetSuccessNotification("News created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _newsService.GetByIdAsync(id);

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification("News not found.");
            return RedirectToAction(nameof(Index));
        }

        var news = result.Data;
        var dto = new NewsCreateDto
        {
            TitleEn = news.TitleEn,
            TitleBn = news.TitleBn,
            ContentEn = news.ContentEn,
            ContentBn = news.ContentBn,
            ImageUrl = news.ImageUrl,
            Category = Enum.TryParse<NewsCategory>(news.Category, out var cat) ? cat : NewsCategory.General,
            PublishDate = news.PublishDate,
            IsActive = news.IsActive,
            IsFeatured = news.IsFeatured
        };

        ViewBag.NewsId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NewsCreateDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.NewsId = id;
            return View(model);
        }

        var updateDto = new NewsUpdateDto
        {
            Id = id,
            TitleEn = model.TitleEn,
            TitleBn = model.TitleBn,
            ContentEn = model.ContentEn,
            ContentBn = model.ContentBn,
            ImageUrl = model.ImageUrl,
            Category = model.Category,
            PublishDate = model.PublishDate,
            IsActive = model.IsActive,
            IsFeatured = model.IsFeatured
        };

        var result = await _newsService.UpdateAsync(updateDto);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update news.");
            ViewBag.NewsId = id;
            return View(model);
        }

        SetSuccessNotification("News updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _newsService.DeleteAsync(id);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to delete news.");
        }
        else
        {
            SetSuccessNotification("News deleted successfully.");
        }

        return RedirectToAction(nameof(Index));
    }
}
