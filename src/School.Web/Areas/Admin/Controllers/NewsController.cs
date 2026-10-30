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
    private readonly IMediaService _mediaService;

    public NewsController(
        INewsService newsService,
        IMediaService mediaService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _newsService = newsService;
        _mediaService = mediaService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _newsService.GetAllAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load news.");
            return View(new List<NewsDto>());
        }

        return View(result.Data.OrderByDescending(n => n.PublishDate).ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new NewsCreateDto { PublishDate = DateTime.Today, Category = NewsCategory.General });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NewsCreateDto model, IFormFile? Image)
    {
        var uploadedUrl = await UploadFileAsync(_mediaService, Image, "news", nameof(model.ImageUrl));
        if (uploadedUrl is not null)
            model.ImageUrl = uploadedUrl;

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
        return View("Create", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NewsCreateDto model, IFormFile? Image, bool removeImage = false)
    {
        var previousImage = model.ImageUrl;
        if (removeImage)
            model.ImageUrl = null;

        var uploadedUrl = await UploadFileAsync(_mediaService, Image, "news", nameof(model.ImageUrl));
        if (uploadedUrl is not null)
            model.ImageUrl = uploadedUrl;

        if (!ModelState.IsValid)
        {
            ViewBag.NewsId = id;
            return View("Create", model);
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
            return View("Create", model);
        }

        if (previousImage != model.ImageUrl)
            await DeleteUploadAsync(_mediaService, previousImage);

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
