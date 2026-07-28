using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Settings;
using School.Application.Services.Interfaces;

namespace School.Web.ViewComponents;

public class FooterInfoViewComponent : ViewComponent
{
    private readonly ISettingsService _settingsService;

    public FooterInfoViewComponent(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await _settingsService.GetSchoolInfoAsync();
        var model = result.Success ? result.Data : null;
        return View(model);
    }
}
