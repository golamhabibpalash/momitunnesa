using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Domain.Enums;

namespace School.Web.Controllers;

public class AchievementController : BaseController
{
    private readonly IAchievementService _achievementService;

    public AchievementController(IAchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    public async Task<IActionResult> Index(AchievementCategory? category)
    {
        var result = await _achievementService.GetAllAsync();

        if (!result.Success || result.Data is null)
            return View(new List<School.Application.DTOs.Achievement.AchievementDto>());

        ViewBag.Category = category?.ToString();
        return View(result.Data
            .Where(a => a.IsActive && (!category.HasValue || a.AchievementCategory == category.Value.ToString()))
            .OrderByDescending(a => a.Date)
            .ToList());
    }
}
