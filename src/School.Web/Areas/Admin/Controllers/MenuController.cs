using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class MenuController : BaseAdminController
{
    private readonly IMenuService _menuService;

    public MenuController(
        IMenuService menuService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _menuService = menuService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _menuService.GetAllAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load menus.");
            return View(new List<MenuDto>());
        }

        return View(result.Data.OrderBy(m => m.SortOrder));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var menusResult = await _menuService.GetAllAsync();
        ViewBag.ParentMenus = menusResult.Success && menusResult.Data is not null
            ? menusResult.Data.Where(m => m.ParentMenuId is null).ToList()
            : new List<MenuDto>();

        return View(new MenuCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MenuCreateDto model)
    {
        if (!ModelState.IsValid)
        {
            var menusResult = await _menuService.GetAllAsync();
            ViewBag.ParentMenus = menusResult.Success && menusResult.Data is not null
                ? menusResult.Data.Where(m => m.ParentMenuId is null).ToList()
                : new List<MenuDto>();
            return View(model);
        }

        var result = await _menuService.CreateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create menu.");
            var menusResult = await _menuService.GetAllAsync();
            ViewBag.ParentMenus = menusResult.Success && menusResult.Data is not null
                ? menusResult.Data.Where(m => m.ParentMenuId is null).ToList()
                : new List<MenuDto>();
            return View(model);
        }

        SetSuccessNotification("Menu created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var menuResult = await _menuService.GetByIdAsync(id);
        var menusResult = await _menuService.GetAllAsync();

        if (!menuResult.Success || menuResult.Data is null)
        {
            SetErrorNotification("Menu not found.");
            return RedirectToAction(nameof(Index));
        }

        var menu = menuResult.Data;
        var dto = new MenuUpdateDto
        {
            Id = menu.Id,
            ParentMenuId = menu.ParentMenuId,
            Title = menu.Title,
            Url = menu.Url,
            Icon = menu.Icon,
            MenuType = Enum.Parse<Domain.Enums.MenuType>(menu.MenuType),
            MenuLocation = Enum.Parse<Domain.Enums.MenuLocation>(menu.MenuLocation),
            SortOrder = menu.SortOrder,
            IsActive = menu.IsActive,
            IsExternal = menu.IsExternal,
            Controller = menu.Controller,
            Action = menu.Action,
            Target = menu.Target
        };

        ViewBag.ParentMenus = menusResult.Success && menusResult.Data is not null
            ? menusResult.Data.Where(m => m.ParentMenuId is null && m.Id != id).ToList()
            : new List<MenuDto>();

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MenuUpdateDto model)
    {
        if (!ModelState.IsValid)
        {
            var menusResult = await _menuService.GetAllAsync();
            ViewBag.ParentMenus = menusResult.Success && menusResult.Data is not null
                ? menusResult.Data.Where(m => m.ParentMenuId is null && m.Id != id).ToList()
                : new List<MenuDto>();
            return View(model);
        }

        model.Id = id;
        var result = await _menuService.UpdateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update menu.");
            var menusResult = await _menuService.GetAllAsync();
            ViewBag.ParentMenus = menusResult.Success && menusResult.Data is not null
                ? menusResult.Data.Where(m => m.ParentMenuId is null && m.Id != id).ToList()
                : new List<MenuDto>();
            return View(model);
        }

        SetSuccessNotification("Menu updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _menuService.DeleteAsync(id);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to delete menu.");
        }
        else
        {
            SetSuccessNotification("Menu deleted successfully.");
        }

        return RedirectToAction(nameof(Index));
    }
}
