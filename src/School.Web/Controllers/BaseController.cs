using Microsoft.AspNetCore.Mvc;
using School.Web.Helpers;

namespace School.Web.Controllers;

public abstract class BaseController : Controller
{
    protected string GetLocalizedContent(string en, string bn)
    {
        return LocalizationHelper.GetLocalizedContent(en, bn);
    }

    protected string GetCurrentLanguage()
    {
        return LocalizationHelper.GetCurrentLanguage();
    }

    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        ViewData["CurrentLanguage"] = GetCurrentLanguage();
    }

    protected IActionResult ServerError(string? message = null)
    {
        message ??= "An unexpected error occurred. Please try again later.";
        return StatusCode(500, new { error = message });
    }

    protected IActionResult NotFoundResponse(string? message = null)
    {
        message ??= "The requested resource was not found.";
        return NotFound(new { error = message });
    }

    protected IActionResult BadRequestResponse(string? message = null)
    {
        message ??= "The request was invalid.";
        return BadRequest(new { error = message });
    }
}
