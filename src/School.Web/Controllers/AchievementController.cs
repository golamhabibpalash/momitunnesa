using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Web.Models;

namespace School.Web.Controllers;

public class AchievementController : BaseController
{
    private readonly IAchievementService _achievementService;

    public AchievementController(IAchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _achievementService.GetAllAsync();

        if (!result.Success || result.Data is null)
            return View(new List<School.Application.DTOs.Achievement.AchievementDto>());

        return View(result.Data);
    }
}
