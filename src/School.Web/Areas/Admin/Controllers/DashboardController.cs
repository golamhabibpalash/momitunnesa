using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Dashboard;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

public class DashboardController : BaseAdminController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(
        IDashboardService dashboardService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _dashboardService.GetDashboardAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load dashboard data.");
            return View(new DashboardDto { ChartYear = DateTime.Now.Year });
        }

        return View(result.Data);
    }
}
