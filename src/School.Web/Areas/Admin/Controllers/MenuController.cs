using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Common;
using School.Application.Services.Interfaces;
using School.Domain.Enums;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class MenuController : BaseAdminController
{
    private readonly IMenuService _menuService;
    private readonly IPageService _pageService;

    public MenuController(
        IMenuService menuService,
        IPageService pageService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _menuService = menuService;
        _pageService = pageService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _menuService.GetAllAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load menus.");
            return View(new List<MenuDto>());
        }

        return View(result.Data.ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reorder([FromBody] List<MenuSortOrderDto> items)
    {
        var result = await _menuService.UpdateSortOrderAsync(items);

        if (!result.Success)
            return Json(new { success = false, message = result.Message ?? "Failed to reorder menus." });

        return Json(new { success = true, message = "Menu order saved successfully." });
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadFormDataAsync(null);
        return View("Edit", new MenuUpdateDto { MenuLocation = MenuLocation.Main, MenuType = MenuType.Internal, IsActive = true, SortOrder = 1 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MenuUpdateDto model)
    {
        NormalizeLink(model);

        if (!ModelState.IsValid)
        {
            await LoadFormDataAsync(null);
            return View("Edit", model);
        }

        var result = await _menuService.CreateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create menu.");
            await LoadFormDataAsync(null);
            return View("Edit", model);
        }

        SetSuccessNotification("Menu created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var menuResult = await _menuService.GetByIdAsync(id);

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
            TitleBn = menu.TitleBn,
            Url = menu.Url,
            Icon = menu.Icon,
            MenuType = Enum.Parse<MenuType>(menu.MenuType),
            MenuLocation = Enum.Parse<MenuLocation>(menu.MenuLocation),
            SortOrder = menu.SortOrder,
            IsActive = menu.IsActive,
            IsExternal = menu.IsExternal,
            Controller = menu.Controller,
            Action = menu.Action,
            Target = menu.Target
        };

        await LoadFormDataAsync(id);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MenuUpdateDto model)
    {
        model.Id = id;
        NormalizeLink(model);

        if (model.ParentMenuId == id)
            ModelState.AddModelError(nameof(model.ParentMenuId), "A menu item cannot be its own parent.");

        if (!ModelState.IsValid)
        {
            await LoadFormDataAsync(id);
            return View(model);
        }

        var result = await _menuService.UpdateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update menu.");
            await LoadFormDataAsync(id);
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
            SetErrorNotification(result.Message ?? "Failed to delete menu.");
        else
            SetSuccessNotification("Menu deleted successfully.");

        return RedirectToAction(nameof(Index));
    }

    private static void NormalizeLink(MenuCreateDto model)
    {
        model.Url = string.IsNullOrWhiteSpace(model.Url) ? null : model.Url.Trim();
        model.IsExternal = model.Url is not null && Uri.TryCreate(model.Url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        model.MenuType = model.IsExternal ? MenuType.External : MenuType.Internal;
    }

    private async Task LoadFormDataAsync(int? currentId)
    {
        var menusResult = await _menuService.GetAllAsync();
        ViewBag.ParentMenus = menusResult.Success && menusResult.Data is not null
            ? menusResult.Data.Where(m => m.ParentMenuId is null && m.Id != currentId).ToList()
            : new List<MenuDto>();

        var pagesResult = await _pageService.GetAllAsync();
        ViewBag.Pages = pagesResult.Success && pagesResult.Data is not null
            ? pagesResult.Data.Where(p => !string.IsNullOrEmpty(p.Slug)).OrderBy(p => p.TitleEn).ToList()
            : new List<School.Application.DTOs.Page.PageDto>();
    }
}
