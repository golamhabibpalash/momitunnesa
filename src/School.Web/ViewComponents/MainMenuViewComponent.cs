using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Domain.Enums;

namespace School.Web.ViewComponents;

public class MainMenuViewComponent : ViewComponent
{
    private readonly IMenuService _menuService;

    public MainMenuViewComponent(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await _menuService.GetMenusByLocationAsync(MenuLocation.Main);
        return View(result.Data ?? Enumerable.Empty<MenuDto>());
    }
}
