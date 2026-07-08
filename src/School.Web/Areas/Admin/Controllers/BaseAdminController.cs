using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Web.Helpers;

namespace School.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public abstract class BaseAdminController : Controller
{
    protected readonly ILogger<BaseAdminController> _logger;
    protected readonly IHttpContextAccessor _httpContextAccessor;

    protected BaseAdminController(ILogger<BaseAdminController> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        ViewData["SiteName"] = "School Management";
        ViewData["CurrentUser"] = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        ViewData["Language"] = LocalizationHelper.GetCurrentLanguage();
        ViewData["LanguageDirection"] = LocalizationHelper.GetLanguageDirection();
    }

    protected void SetSuccessNotification(string message)
    {
        TempData["SuccessMessage"] = message;
    }

    protected void SetErrorNotification(string message)
    {
        TempData["ErrorMessage"] = message;
    }

    protected void SetInfoNotification(string message)
    {
        TempData["InfoMessage"] = message;
    }

    protected void SetWarningNotification(string message)
    {
        TempData["WarningMessage"] = message;
    }

    protected IActionResult NotFound(string resourceName = "Resource")
    {
        _logger.LogWarning("{ResourceName} not found.", resourceName);
        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }

    protected static void ThrowIfNull(object? obj, string paramName)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
