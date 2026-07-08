using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Achievement;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class AchievementController : BaseAdminController
{
    private readonly IAchievementService _achievementService;

    public AchievementController(
        IAchievementService achievementService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _achievementService = achievementService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _achievementService.GetAllAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load achievements.");
            return View(new List<AchievementDto>());
        }

        return View(result.Data.OrderByDescending(a => a.Date));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new AchievementDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AchievementDto model)
    {
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
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AchievementDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AchievementId = id;
            return View(model);
        }

        model.Id = id;
        var result = await _achievementService.UpdateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update achievement.");
            ViewBag.AchievementId = id;
            return View(model);
        }

        SetSuccessNotification("Achievement updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _achievementService.DeleteAsync(id);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to delete achievement.");
        }
        else
        {
            SetSuccessNotification("Achievement deleted successfully.");
        }

        return RedirectToAction(nameof(Index));
    }
}
