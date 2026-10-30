using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Event;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class EventController : BaseAdminController
{
    private readonly IEventService _eventService;
    private readonly IMediaService _mediaService;

    public EventController(
        IEventService eventService,
        IMediaService mediaService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _eventService = eventService;
        _mediaService = mediaService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _eventService.GetAllAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load events.");
            return View(new List<EventDto>());
        }

        return View(result.Data.OrderByDescending(e => e.EventDate).ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new EventCreateDto { EventDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventCreateDto model, IFormFile? Image)
    {
        var uploadedUrl = await UploadFileAsync(_mediaService, Image, "events", nameof(model.ImageUrl));
        if (uploadedUrl is not null)
            model.ImageUrl = uploadedUrl;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _eventService.CreateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create event.");
            return View(model);
        }

        SetSuccessNotification("Event created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _eventService.GetByIdAsync(id);

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification("Event not found.");
            return RedirectToAction(nameof(Index));
        }

        var evt = result.Data;
        var dto = new EventCreateDto
        {
            TitleEn = evt.TitleEn,
            TitleBn = evt.TitleBn,
            DescriptionEn = evt.DescriptionEn,
            DescriptionBn = evt.DescriptionBn,
            ImageUrl = evt.ImageUrl,
            EventDate = evt.EventDate,
            StartTime = evt.StartTime,
            EndTime = evt.EndTime,
            Location = evt.Location,
            IsActive = evt.IsActive,
            IsFeatured = evt.IsFeatured
        };

        ViewBag.EventId = id;
        return View("Create", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EventCreateDto model, IFormFile? Image, bool removeImage = false)
    {
        var previousImage = model.ImageUrl;
        if (removeImage)
            model.ImageUrl = null;

        var uploadedUrl = await UploadFileAsync(_mediaService, Image, "events", nameof(model.ImageUrl));
        if (uploadedUrl is not null)
            model.ImageUrl = uploadedUrl;

        if (!ModelState.IsValid)
        {
            ViewBag.EventId = id;
            return View("Create", model);
        }

        var updateDto = new EventUpdateDto
        {
            Id = id,
            TitleEn = model.TitleEn,
            TitleBn = model.TitleBn,
            DescriptionEn = model.DescriptionEn,
            DescriptionBn = model.DescriptionBn,
            ImageUrl = model.ImageUrl,
            EventDate = model.EventDate,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            Location = model.Location,
            IsActive = model.IsActive,
            IsFeatured = model.IsFeatured
        };

        var result = await _eventService.UpdateAsync(updateDto);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update event.");
            ViewBag.EventId = id;
            return View("Create", model);
        }

        if (previousImage != model.ImageUrl)
            await DeleteUploadAsync(_mediaService, previousImage);

        SetSuccessNotification("Event updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _eventService.DeleteAsync(id);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to delete event.");
        }
        else
        {
            SetSuccessNotification("Event deleted successfully.");
        }

        return RedirectToAction(nameof(Index));
    }
}
