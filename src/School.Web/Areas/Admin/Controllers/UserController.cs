using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Account;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class UserController : BaseAdminController
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private const int PageSize = 20;

    public UserController(
        IUserService userService,
        IRoleService roleService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _userService = userService;
        _roleService = roleService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _userService.GetAllAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load users.");
            return View(new List<UserDto>());
        }

        var users = result.Data.ToList();
        var totalPages = (int)Math.Ceiling(users.Count / (double)PageSize);
        var pagedItems = users.Skip((page - 1) * PageSize).Take(PageSize).ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(pagedItems);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var rolesResult = await _roleService.GetAllAsync();
        ViewBag.Roles = rolesResult.Success && rolesResult.Data is not null
            ? rolesResult.Data
            : new List<RoleDto>();

        return View(new UserCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateDto model)
    {
        if (!ModelState.IsValid)
        {
            var rolesResult = await _roleService.GetAllAsync();
            ViewBag.Roles = rolesResult.Success && rolesResult.Data is not null
                ? rolesResult.Data
                : new List<RoleDto>();
            return View(model);
        }

        var result = await _userService.CreateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create user.");
            var rolesResult = await _roleService.GetAllAsync();
            ViewBag.Roles = rolesResult.Success && rolesResult.Data is not null
                ? rolesResult.Data
                : new List<RoleDto>();
            return View(model);
        }

        SetSuccessNotification("User created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var userResult = await _userService.GetByIdAsync(id);
        var rolesResult = await _roleService.GetAllAsync();

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
        ViewBag.AllRoles = rolesResult.Success && rolesResult.Data is not null
            ? rolesResult.Data
            : new List<RoleDto>();

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UserUpdateDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.UserId = id;
            var rolesResult = await _roleService.GetAllAsync();
            ViewBag.AllRoles = rolesResult.Success && rolesResult.Data is not null
                ? rolesResult.Data
                : new List<RoleDto>();
            return View(model);
        }

        var result = await _userService.UpdateAsync(id, model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update user.");
            ViewBag.UserId = id;
            var rolesResult = await _roleService.GetAllAsync();
            ViewBag.AllRoles = rolesResult.Success && rolesResult.Data is not null
                ? rolesResult.Data
                : new List<RoleDto>();
            return View(model);
        }

        SetSuccessNotification("User updated successfully.");
        return RedirectToAction(nameof(Index));
    }
}
