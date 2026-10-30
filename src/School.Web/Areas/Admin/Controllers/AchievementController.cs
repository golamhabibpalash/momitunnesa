using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Achievement;
using School.Application.Services.Interfaces;
using School.Domain.Enums;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class AchievementController : BaseAdminController
{
    private readonly IAchievementService _achievementService;
    private readonly IMediaService _mediaService;

    public AchievementController(
        IAchievementService achievementService,
        IMediaService mediaService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _achievementService = achievementService;
        _mediaService = mediaService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _achievementService.GetAllAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load achievements.");
            return View(new List<AchievementDto>());
        }

        return View(result.Data.OrderByDescending(a => a.Date).ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new AchievementDto { Date = DateTime.Today, IsActive = true, AchievementCategory = nameof(AchievementCategory.Student) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AchievementDto model, IFormFile? Image)
    {
        ValidateCategory(model);
        var uploadedUrl = await UploadFileAsync(_mediaService, Image, "achievements", nameof(model.ImageUrl));
        if (uploadedUrl is not null)
            model.ImageUrl = uploadedUrl;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _achievementService.CreateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create achievement.");
            return View(model);
        }

        SetSuccessNotification("Achievement created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _achievementService.GetByIdAsync(id);

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification("Achievement not found.");
            return RedirectToAction(nameof(Index));
        }

        ViewBag.AchievementId = id;
        return View("Create", result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AchievementDto model, IFormFile? Image, bool removeImage = false)
    {
        ValidateCategory(model);
        var previousImage = model.ImageUrl;
        if (removeImage)
            model.ImageUrl = null;

        var uploadedUrl = await UploadFileAsync(_mediaService, Image, "achievements", nameof(model.ImageUrl));
        if (uploadedUrl is not null)
            model.ImageUrl = uploadedUrl;

        if (!ModelState.IsValid)
        {
            ViewBag.AchievementId = id;
            return View("Create", model);
        }

        model.Id = id;
        var result = await _achievementService.UpdateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update achievement.");
            ViewBag.AchievementId = id;
            return View("Create", model);
        }

        if (previousImage != model.ImageUrl)
            await DeleteUploadAsync(_mediaService, previousImage);

        SetSuccessNotification("Achievement updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _achievementService.DeleteAsync(id);

        if (!result.Success)
            SetErrorNotification(result.Message ?? "Failed to delete achievement.");
        else
            SetSuccessNotification("Achievement deleted successfully.");

        return RedirectToAction(nameof(Index));
    }

    private void ValidateCategory(AchievementDto model)
    {
        // The Bangla title is optional on this form; the public page falls back to English.
        ModelState.Remove(nameof(model.TitleBn));
        model.TitleBn ??= string.Empty;

        if (string.IsNullOrWhiteSpace(model.TitleEn))
            ModelState.AddModelError(nameof(model.TitleEn), "Title (English) is required.");
        if (!Enum.TryParse<AchievementCategory>(model.AchievementCategory, out _))
            ModelState.AddModelError(nameof(model.AchievementCategory), "Please choose a valid category.");
    }
}
