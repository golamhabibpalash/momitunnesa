using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Account;
using School.Application.DTOs.Common;
using School.Application.Services.Interfaces;
using School.Domain.Entities;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class UserController : BaseAdminController
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(
        IUserService userService,
        IRoleService roleService,
        UserManager<ApplicationUser> userManager,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _userService = userService;
        _roleService = roleService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _userService.GetAllAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load users.");
            return View(new List<UserDto>());
        }

        ViewBag.CurrentUserId = _userManager.GetUserId(User);
        return View(result.Data.OrderBy(u => u.Email).ToList());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadRolesAsync();
        return View(new UserCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateDto model)
    {
        if (!ModelState.IsValid)
        {
            await LoadRolesAsync();
            return View(model);
        }

        var result = await _userService.CreateAsync(model);

        if (!result.Success)
        {
            AddErrors(result);
            await LoadRolesAsync();
            return View(model);
        }

        SetSuccessNotification("User created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var userResult = await _userService.GetByIdAsync(id);

        if (!userResult.Success || userResult.Data is null)
        {
            SetErrorNotification("User not found.");
            return RedirectToAction(nameof(Index));
        }

        var user = userResult.Data;
        var dto = new UserUpdateDto
        {
            FullName = user.FullName ?? "",
            Phone = user.Phone,
            IsActive = user.IsActive,
            Roles = user.Roles.ToList()
        };

        ViewBag.UserId = id;
        ViewBag.UserEmail = user.Email;
        await LoadRolesAsync();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UserUpdateDto model)
    {
        ViewBag.UserId = id;
        ViewBag.UserEmail = (await _userService.GetByIdAsync(id)).Data?.Email;

        if (id == _userManager.GetUserId(User) && (!model.IsActive || !model.Roles.Contains("SuperAdmin")))
            ModelState.AddModelError(string.Empty, "You cannot deactivate your own account or remove your own SuperAdmin role.");

        if (!ModelState.IsValid)
        {
            await LoadRolesAsync();
            return View(model);
        }

        var result = await _userService.UpdateAsync(id, model);

        if (!result.Success)
        {
            AddErrors(result);
            await LoadRolesAsync();
            return View(model);
        }

        SetSuccessNotification("User updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        if (id == _userManager.GetUserId(User))
        {
            SetErrorNotification("You cannot delete your own account.");
            return RedirectToAction(nameof(Index));
        }

        var result = await _userService.DeleteAsync(id);

        if (!result.Success)
            SetErrorNotification(result.Message ?? "Failed to delete user.");
        else
            SetSuccessNotification("User deleted successfully.");

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadRolesAsync()
    {
        var rolesResult = await _roleService.GetAllAsync();
        ViewBag.Roles = rolesResult.Success && rolesResult.Data is not null
            ? rolesResult.Data.ToList()
            : new List<RoleDto>();
    }

    private void AddErrors<T>(ResponseDto<T> result)
    {
        ModelState.AddModelError(string.Empty, result.Message ?? "The operation failed.");
        foreach (var error in result.Errors ?? new List<string>())
            ModelState.AddModelError(string.Empty, error);
    }
}
