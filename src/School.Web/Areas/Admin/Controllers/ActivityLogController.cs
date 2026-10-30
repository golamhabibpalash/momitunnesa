using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class ActivityLogController : BaseAdminController
{
    private const int PageSize = 50;
    private readonly IApplicationDataService _dataService;

    public ActivityLogController(
        IApplicationDataService dataService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _dataService = dataService;
    }

    public async Task<IActionResult> Index(int page = 1, string? search = null)
    {
        var (items, total) = await _dataService.GetActivityLogsAsync(page, PageSize, search);
        ViewBag.CurrentPage = Math.Max(page, 1);
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)PageSize);
        ViewBag.TotalCount = total;
        ViewBag.Search = search;
        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear(int olderThanDays = 90)
    {
        var deleted = await _dataService.DeleteActivityLogsOlderThanAsync(DateTime.UtcNow.AddDays(-Math.Max(olderThanDays, 0)));
        SetSuccessNotification($"{deleted} log entries older than {olderThanDays} days were removed.");
        return RedirectToAction(nameof(Index));
    }
}
