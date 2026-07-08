using Microsoft.AspNetCore.Mvc;
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
        return View(result.Data);
    }
}
