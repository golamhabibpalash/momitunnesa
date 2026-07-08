using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Domain.Enums;

namespace School.Web.ViewComponents;

public class FooterMenuViewComponent : ViewComponent
{
    private readonly IMenuService _menuService;

    public FooterMenuViewComponent(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await _menuService.GetMenusByLocationAsync(MenuLocation.Footer);
        return View(result.Data ?? Enumerable.Empty<MenuDto>());
    }
}
