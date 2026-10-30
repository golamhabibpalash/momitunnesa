using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Page;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class PageController : BaseAdminController
{
    private readonly IPageService _pageService;
    private readonly IMediaService _mediaService;

    public PageController(
        IPageService pageService,
        IMediaService mediaService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _pageService = pageService;
        _mediaService = mediaService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _pageService.GetAllAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load pages.");
            return View(new List<PageDto>());
        }

        return View(result.Data.OrderByDescending(p => p.CreatedAt).ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new PageCreateDto { IsPublished = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PageCreateDto model, IFormFile? FeaturedImageFile)
    {
        if (string.IsNullOrWhiteSpace(model.TitleEn) && string.IsNullOrWhiteSpace(model.TitleBn))
            ModelState.AddModelError(nameof(model.TitleEn), "A title in English or Bangla is required.");

        var uploadedUrl = await UploadFileAsync(_mediaService, FeaturedImageFile, "pages", nameof(model.FeaturedImage));
        if (uploadedUrl is not null)
            model.FeaturedImage = uploadedUrl;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _pageService.CreateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create page.");
            return View(model);
        }

        SetSuccessNotification("Page created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _pageService.GetByIdAsync(id);

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification("Page not found.");
            return RedirectToAction(nameof(Index));
        }

        var page = result.Data;
        var dto = new PageCreateDto
        {
            TitleEn = page.TitleEn,
            TitleBn = page.TitleBn,
            ContentEn = page.ContentEn,
            ContentBn = page.ContentBn,
            FeaturedImage = page.FeaturedImage,
            MetaTitle = page.MetaTitle,
            MetaDescription = page.MetaDescription,
            MetaKeywords = page.MetaKeywords,
            IsHomePage = page.IsHomePage,
            IsPublished = page.IsPublished,
            Status = page.Status,
            TemplateType = page.TemplateType,
            SortOrder = page.SortOrder
        };

        ViewBag.PageId = id;
        ViewBag.PageSlug = page.Slug;
        return View("Create", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PageCreateDto model, IFormFile? FeaturedImageFile, bool removeImage = false)
    {
        if (string.IsNullOrWhiteSpace(model.TitleEn) && string.IsNullOrWhiteSpace(model.TitleBn))
            ModelState.AddModelError(nameof(model.TitleEn), "A title in English or Bangla is required.");

        var previousImage = model.FeaturedImage;
        if (removeImage)
            model.FeaturedImage = null;

        var uploadedUrl = await UploadFileAsync(_mediaService, FeaturedImageFile, "pages", nameof(model.FeaturedImage));
        if (uploadedUrl is not null)
            model.FeaturedImage = uploadedUrl;

        if (!ModelState.IsValid)
        {
            ViewBag.PageId = id;
            return View("Create", model);
        }

        var updateDto = new PageUpdateDto
        {
            Id = id,
            TitleEn = model.TitleEn,
            TitleBn = model.TitleBn,
            ContentEn = model.ContentEn,
            ContentBn = model.ContentBn,
            FeaturedImage = model.FeaturedImage,
            MetaTitle = model.MetaTitle,
            MetaDescription = model.MetaDescription,
            MetaKeywords = model.MetaKeywords,
            IsHomePage = model.IsHomePage,
            IsPublished = model.IsPublished,
            Status = model.Status,
            TemplateType = model.TemplateType,
            SortOrder = model.SortOrder
        };

        var result = await _pageService.UpdateAsync(updateDto);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update page.");
            ViewBag.PageId = id;
            return View("Create", model);
        }

        if (previousImage != model.FeaturedImage)
            await DeleteUploadAsync(_mediaService, previousImage);

        SetSuccessNotification("Page updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _pageService.DeleteAsync(id);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to delete page.");
        }
        else
        {
            SetSuccessNotification("Page deleted successfully.");
        }

        return RedirectToAction(nameof(Index));
    }
}
