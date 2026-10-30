using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Academic;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class SubjectController : BaseAdminController
{
    private readonly IAcademicService _academicService;

    public SubjectController(
        IAcademicService academicService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _academicService = academicService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _academicService.GetSubjectsAsync();
        if (!result.Success)
            SetErrorNotification(result.Message ?? "Failed to load subjects.");

        return View(result.Data?.ToList() ?? new List<SubjectDto>());
    }

    [HttpGet]
    public IActionResult Create() => View("Form", new SubjectUpdateDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubjectUpdateDto model)
    {
        ModelState.Remove(nameof(model.Id));
        if (!ModelState.IsValid)
            return View("Form", model);

        var result = await _academicService.CreateSubjectAsync(model);
        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to save the subject.");
            return View("Form", model);
        }

        SetSuccessNotification("Subject added.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _academicService.GetSubjectByIdAsync(id);
        if (result.Data is not { } item)
        {
            SetErrorNotification("Subject not found.");
            return RedirectToAction(nameof(Index));
        }

        return View("Form", new SubjectUpdateDto
        {
            Id = item.Id,
            NameEn = item.NameEn,
            NameBn = item.NameBn,
            Code = item.Code,
            ClassName = item.ClassName,
            IsActive = item.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SubjectUpdateDto model)
    {
        model.Id = id;
        if (!ModelState.IsValid)
            return View("Form", model);

        var result = await _academicService.UpdateSubjectAsync(model);
        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update the subject.");
            return View("Form", model);
        }

        SetSuccessNotification("Subject updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _academicService.DeleteSubjectAsync(id);
        if (result.Success) SetSuccessNotification("Subject deleted.");
        else SetErrorNotification(result.Message ?? "Failed to delete the subject.");
        return RedirectToAction(nameof(Index));
    }
}
