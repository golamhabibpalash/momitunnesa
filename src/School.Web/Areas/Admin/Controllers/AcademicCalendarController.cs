using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Academic;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class AcademicCalendarController : BaseAdminController
{
    private readonly IAcademicService _academicService;

    public AcademicCalendarController(
        IAcademicService academicService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _academicService = academicService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _academicService.GetCalendarEventsAsync();
        if (!result.Success)
            SetErrorNotification(result.Message ?? "Failed to load the academic calendar.");

        return View(result.Data?.ToList() ?? new List<AcademicCalendarDto>());
    }

    [HttpGet]
    public IActionResult Create() => View("Form", new AcademicCalendarUpdateDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AcademicCalendarUpdateDto model)
    {
        ModelState.Remove(nameof(model.Id));
        if (!ModelState.IsValid)
            return View("Form", model);

        var result = await _academicService.CreateCalendarEventAsync(model);
        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to save the calendar event.");
            return View("Form", model);
        }

        SetSuccessNotification("Calendar event added.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _academicService.GetCalendarEventByIdAsync(id);
        if (result.Data is not { } item)
        {
            SetErrorNotification("Calendar event not found.");
            return RedirectToAction(nameof(Index));
        }

        return View("Form", new AcademicCalendarUpdateDto
        {
            Id = item.Id,
            TitleEn = item.TitleEn,
            TitleBn = item.TitleBn,
            DescriptionEn = item.DescriptionEn,
            DescriptionBn = item.DescriptionBn,
            EventDate = item.EventDate,
            IsActive = item.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AcademicCalendarUpdateDto model)
    {
        model.Id = id;
        if (!ModelState.IsValid)
            return View("Form", model);

        var result = await _academicService.UpdateCalendarEventAsync(model);
        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update the calendar event.");
            return View("Form", model);
        }

        SetSuccessNotification("Calendar event updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _academicService.DeleteCalendarEventAsync(id);
        if (result.Success) SetSuccessNotification("Calendar event deleted.");
        else SetErrorNotification(result.Message ?? "Failed to delete the calendar event.");
        return RedirectToAction(nameof(Index));
    }
}
