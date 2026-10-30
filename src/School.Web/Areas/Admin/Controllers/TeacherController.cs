using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Teacher;
using School.Application.Services.Interfaces;
using School.Domain.Enums;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class TeacherController : BaseAdminController
{
    private readonly ITeacherService _teacherService;
    private readonly IMediaService _mediaService;

    public TeacherController(
        ITeacherService teacherService,
        IMediaService mediaService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _teacherService = teacherService;
        _mediaService = mediaService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _teacherService.GetAllAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load teachers.");
            return View(new List<TeacherDto>());
        }

        return View(result.Data.OrderBy(t => t.SortOrder).ThenBy(t => t.NameEn).ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new TeacherCreateDto { Designation = Designation.JuniorTeacher });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TeacherCreateDto model, IFormFile? Photo)
    {
        var uploadedUrl = await UploadFileAsync(_mediaService, Photo, "teachers", nameof(model.ImageUrl));
        if (uploadedUrl is not null)
            model.ImageUrl = uploadedUrl;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _teacherService.CreateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create teacher.");
            return View(model);
        }

        SetSuccessNotification("Teacher created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _teacherService.GetByIdAsync(id);

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification("Teacher not found.");
            return RedirectToAction(nameof(Index));
        }

        var teacher = result.Data;
        var dto = new TeacherCreateDto
        {
            NameEn = teacher.NameEn,
            NameBn = teacher.NameBn,
            Designation = Enum.TryParse<Designation>(teacher.Designation, out var des) ? des : Designation.JuniorTeacher,
            ImageUrl = teacher.ImageUrl,
            Email = teacher.Email,
            Phone = teacher.Phone,
            QualificationEn = teacher.QualificationEn,
            QualificationBn = teacher.QualificationBn,
            SubjectEn = teacher.SubjectEn,
            SubjectBn = teacher.SubjectBn,
            JoiningDate = teacher.JoiningDate,
            IsActive = teacher.IsActive,
            SortOrder = teacher.SortOrder,
            IsHeadTeacher = teacher.IsHeadTeacher,
            IsAssistantHeadTeacher = teacher.IsAssistantHeadTeacher
        };

        ViewBag.TeacherId = id;
        return View("Create", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TeacherCreateDto model, IFormFile? Photo, bool removeImage = false)
    {
        var previousImage = model.ImageUrl;
        if (removeImage)
            model.ImageUrl = null;

        var uploadedUrl = await UploadFileAsync(_mediaService, Photo, "teachers", nameof(model.ImageUrl));
        if (uploadedUrl is not null)
            model.ImageUrl = uploadedUrl;

        if (!ModelState.IsValid)
        {
            ViewBag.TeacherId = id;
            return View("Create", model);
        }

        var updateDto = new TeacherUpdateDto
        {
            Id = id,
            NameEn = model.NameEn,
            NameBn = model.NameBn,
            Designation = model.Designation,
            ImageUrl = model.ImageUrl,
            Email = model.Email,
            Phone = model.Phone,
            QualificationEn = model.QualificationEn,
            QualificationBn = model.QualificationBn,
            SubjectEn = model.SubjectEn,
            SubjectBn = model.SubjectBn,
            JoiningDate = model.JoiningDate,
            IsActive = model.IsActive,
            SortOrder = model.SortOrder,
            IsHeadTeacher = model.IsHeadTeacher,
            IsAssistantHeadTeacher = model.IsAssistantHeadTeacher
        };

        var result = await _teacherService.UpdateAsync(updateDto);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update teacher.");
            ViewBag.TeacherId = id;
            return View("Create", model);
        }

        if (previousImage != model.ImageUrl)
            await DeleteUploadAsync(_mediaService, previousImage);

        SetSuccessNotification("Teacher updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _teacherService.DeleteAsync(id);

        if (!result.Success)
            SetErrorNotification(result.Message ?? "Failed to delete teacher.");
        else
            SetSuccessNotification("Teacher deleted successfully.");

        return RedirectToAction(nameof(Index));
    }
}
