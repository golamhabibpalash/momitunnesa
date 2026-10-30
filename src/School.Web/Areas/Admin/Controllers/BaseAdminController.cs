using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
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

    /// <summary>
    /// Records every successful admin POST (create, update, delete, upload...) in the activity log.
    /// A request counts as successful when it redirects and did not set an error notification.
    /// </summary>
    public override async Task OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context, Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate next)
    {
        var executed = await next();

        var request = HttpContext.Request;
        if (!HttpMethods.IsPost(request.Method) || executed.Exception is not null || User.Identity?.IsAuthenticated != true)
            return;
        if (executed.Result is not (RedirectToActionResult or RedirectResult or JsonResult) || TempData.Peek("ErrorMessage") is not null)
            return;

        var controller = context.RouteData.Values["controller"]?.ToString();
        var action = context.RouteData.Values["action"]?.ToString();
        if (controller == "Account" && action == "Logout")
            return;

        try
        {
            var dataService = HttpContext.RequestServices.GetRequiredService<IApplicationDataService>();
            var id = context.RouteData.Values["id"]?.ToString();
            var message = TempData.Peek("SuccessMessage") as string;
            await dataService.LogActivityAsync(new School.Domain.Entities.ActivityLog
            {
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                UserName = User.Identity?.Name,
                Action = action,
                Controller = controller,
                Description = string.IsNullOrEmpty(message) ? $"{action} {controller}{(id is null ? "" : $" #{id}")}" : message + (id is null ? "" : $" (#{id})"),
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                BrowserInfo = request.Headers.UserAgent.ToString() is { Length: > 0 } ua ? ua[..Math.Min(ua.Length, 500)] : null,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            // Logging must never break the admin action itself.
            _logger.LogWarning(ex, "Failed to write activity log for {Controller}/{Action}.", controller, action);
        }
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

    /// <summary>
    /// Uploads an optional form file. Returns the new URL, or null when no file was sent
    /// or the upload failed (the failure is added to ModelState under <paramref name="modelKey"/>).
    /// </summary>
    protected async Task<string?> UploadFileAsync(IMediaService mediaService, IFormFile? file, string folder, string modelKey)
    {
        if (file is null || file.Length == 0)
            return null;

        var result = await mediaService.UploadAsync(file, folder);
        if (result.Success)
            return result.Url;

        ModelState.AddModelError(modelKey, result.Error ?? "Failed to upload file.");
        return null;
    }

    /// <summary>Deletes a previously uploaded file. Only paths under /uploads/ are touched.</summary>
    protected static async Task DeleteUploadAsync(IMediaService mediaService, string? url)
    {
        if (!string.IsNullOrEmpty(url) && url.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            await mediaService.DeleteAsync(url);
    }

    protected static void ThrowIfNull(object? obj, string paramName)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
