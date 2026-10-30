using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Academic;
using School.Application.Services.Interfaces;
using School.Web.Helpers;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class RoutineController : BaseAdminController
{
    private readonly IAcademicService _academicService;

    public RoutineController(
        IAcademicService academicService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _academicService = academicService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _academicService.GetRoutinesAsync();
        if (!result.Success)
            SetErrorNotification(result.Message ?? "Failed to load the routine.");

        return View(result.Data?.ToList() ?? new List<RoutineDto>());
    }

    [HttpGet]
    public IActionResult Create(string? classNameEn, string? classNameBn, string? section, string? dayOfWeek)
    {
        // Prefill from the previous entry so a whole day's routine can be entered quickly.
        return View("Form", new RoutineUpdateDto
        {
            ClassNameEn = classNameEn ?? "",
            ClassNameBn = classNameBn ?? "",
            Section = section,
            DayOfWeek = dayOfWeek ?? "Sunday"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoutineUpdateDto model, bool addAnother = false)
    {
        ModelState.Remove(nameof(model.Id));
        Validate(model);
        if (!ModelState.IsValid)
            return View("Form", model);

        var result = await _academicService.CreateRoutineAsync(model);
        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to save the routine entry.");
            return View("Form", model);
        }

        SetSuccessNotification("Routine entry added.");
        return addAnother
            ? RedirectToAction(nameof(Create), new { model.ClassNameEn, model.ClassNameBn, model.Section, model.DayOfWeek })
            : RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _academicService.GetRoutineByIdAsync(id);
        if (result.Data is not { } item)
        {
            SetErrorNotification("Routine entry not found.");
            return RedirectToAction(nameof(Index));
        }

        return View("Form", new RoutineUpdateDto
        {
            Id = item.Id,
            ClassNameEn = item.ClassNameEn,
            ClassNameBn = item.ClassNameBn,
            Section = item.Section,
            DayOfWeek = item.DayOfWeek,
            SubjectEn = item.SubjectEn,
            SubjectBn = item.SubjectBn,
            TeacherEn = item.TeacherEn,
            TeacherBn = item.TeacherBn,
            StartTime = item.StartTime,
            EndTime = item.EndTime,
            Room = item.Room,
            IsActive = item.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RoutineUpdateDto model)
    {
        model.Id = id;
        Validate(model);
        if (!ModelState.IsValid)
            return View("Form", model);

        var result = await _academicService.UpdateRoutineAsync(model);
        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update the routine entry.");
            return View("Form", model);
        }

        SetSuccessNotification("Routine entry updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _academicService.DeleteRoutineAsync(id);
        if (result.Success) SetSuccessNotification("Routine entry deleted.");
        else SetErrorNotification(result.Message ?? "Failed to delete the routine entry.");
        return RedirectToAction(nameof(Index));
    }

    private void Validate(RoutineUpdateDto model)
    {
        if (model.EndTime <= model.StartTime)
            ModelState.AddModelError(nameof(model.EndTime), "End time must be after the start time.");
        if (!AcademicOptions.Days.Contains(model.DayOfWeek))
            ModelState.AddModelError(nameof(model.DayOfWeek), "Choose a valid day.");
    }
}
