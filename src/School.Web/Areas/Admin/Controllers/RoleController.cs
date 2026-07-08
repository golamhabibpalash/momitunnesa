using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Account;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class RoleController : BaseAdminController
{
    private readonly IRoleService _roleService;

    public RoleController(
        IRoleService roleService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _roleService = roleService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _roleService.GetAllAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load roles.");
            return View(new List<RoleDto>());
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoleCreateDto model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
        {
            SetErrorNotification("Role name is required.");
            return RedirectToAction(nameof(Index));
        }

        var result = await _roleService.CreateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create role.");
        }
        else
        {
            SetSuccessNotification("Role created successfully.");
        }

        return RedirectToAction(nameof(Index));
    }
}
