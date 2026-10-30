using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Staff;
using School.Application.Services.Interfaces;
using School.Domain.Enums;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class StaffController : BaseAdminController
{
    private readonly IStaffService _staffService;
    private readonly IMediaService _mediaService;

    public StaffController(
        IStaffService staffService,
        IMediaService mediaService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _staffService = staffService;
        _mediaService = mediaService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _staffService.GetAllAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load staff.");
            return View(new List<StaffDto>());
        }

        return View(result.Data.OrderBy(s => s.SortOrder).ThenBy(s => s.NameEn).ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new StaffCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StaffCreateDto model, IFormFile? Photo)
    {
        model.Designation = Designation.Staff;
        var uploadedUrl = await UploadFileAsync(_mediaService, Photo, "staff", nameof(model.ImageUrl));
        if (uploadedUrl is not null)
            model.ImageUrl = uploadedUrl;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _staffService.CreateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create staff.");
            return View(model);
        }

        SetSuccessNotification("Staff created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _staffService.GetByIdAsync(id);

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification("Staff not found.");
            return RedirectToAction(nameof(Index));
        }

        var staff = result.Data;
        var dto = new StaffCreateDto
        {
            NameEn = staff.NameEn,
            NameBn = staff.NameBn,
            PositionEn = staff.PositionEn,
            PositionBn = staff.PositionBn,
            ImageUrl = staff.ImageUrl,
            Email = staff.Email,
            Phone = staff.Phone,
            Department = staff.Department,
            JoiningDate = staff.JoiningDate,
            IsActive = staff.IsActive,
            SortOrder = staff.SortOrder
        };

        ViewBag.StaffId = id;
        return View("Create", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StaffCreateDto model, IFormFile? Photo, bool removeImage = false)
    {
        var previousImage = model.ImageUrl;
        if (removeImage)
            model.ImageUrl = null;

        var uploadedUrl = await UploadFileAsync(_mediaService, Photo, "staff", nameof(model.ImageUrl));
        if (uploadedUrl is not null)
            model.ImageUrl = uploadedUrl;

        if (!ModelState.IsValid)
        {
            ViewBag.StaffId = id;
            return View("Create", model);
        }

        var updateDto = new StaffUpdateDto
        {
            Id = id,
            NameEn = model.NameEn,
            NameBn = model.NameBn,
            Designation = Designation.Staff,
            PositionEn = model.PositionEn,
            PositionBn = model.PositionBn,
            ImageUrl = model.ImageUrl,
            Email = model.Email,
            Phone = model.Phone,
            Department = model.Department,
            JoiningDate = model.JoiningDate,
            IsActive = model.IsActive,
            SortOrder = model.SortOrder
        };

        var result = await _staffService.UpdateAsync(updateDto);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update staff.");
            ViewBag.StaffId = id;
            return View("Create", model);
        }

        if (previousImage != model.ImageUrl)
            await DeleteUploadAsync(_mediaService, previousImage);

        SetSuccessNotification("Staff updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _staffService.DeleteAsync(id);

        if (!result.Success)
            SetErrorNotification(result.Message ?? "Failed to delete staff.");
        else
            SetSuccessNotification("Staff deleted successfully.");

        return RedirectToAction(nameof(Index));
    }
}
