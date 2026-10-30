using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Academic;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class SyllabusController : BaseAdminController
{
    private readonly IAcademicService _academicService;
    private readonly IMediaService _mediaService;

    public SyllabusController(
        IAcademicService academicService,
        IMediaService mediaService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _academicService = academicService;
        _mediaService = mediaService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _academicService.GetSyllabusesAsync();
        if (!result.Success)
            SetErrorNotification(result.Message ?? "Failed to load syllabus files.");

        return View(result.Data?.ToList() ?? new List<SyllabusDto>());
    }

    [HttpGet]
    public IActionResult Create() => View("Form", new SyllabusUpdateDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SyllabusUpdateDto model, IFormFile? File)
    {
        ModelState.Remove(nameof(model.Id));
        await ApplyFileAsync(model, File);
        if (!ModelState.IsValid)
            return View("Form", model);

        var result = await _academicService.CreateSyllabusAsync(model);
        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to save the syllabus.");
            return View("Form", model);
        }

        SetSuccessNotification("Syllabus added.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _academicService.GetSyllabusByIdAsync(id);
        if (result.Data is not { } item)
        {
            SetErrorNotification("Syllabus not found.");
            return RedirectToAction(nameof(Index));
        }

        return View("Form", new SyllabusUpdateDto
        {
            Id = item.Id,
            TitleEn = item.TitleEn,
            TitleBn = item.TitleBn,
            ClassName = item.ClassName,
            Subject = item.Subject,
            FileUrl = item.FileUrl,
            FileType = item.FileType,
            IsActive = item.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SyllabusUpdateDto model, IFormFile? File)
    {
        model.Id = id;
        var previousFile = model.FileUrl;
        await ApplyFileAsync(model, File);
        if (!ModelState.IsValid)
            return View("Form", model);

        var result = await _academicService.UpdateSyllabusAsync(model);
        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update the syllabus.");
            return View("Form", model);
        }

        if (previousFile != model.FileUrl)
            await DeleteUploadAsync(_mediaService, previousFile);

        SetSuccessNotification("Syllabus updated.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = (await _academicService.GetSyllabusByIdAsync(id)).Data;
        var result = await _academicService.DeleteSyllabusAsync(id);
        if (result.Success)
        {
            await DeleteUploadAsync(_mediaService, item?.FileUrl);
            SetSuccessNotification("Syllabus deleted.");
        }
        else
        {
            SetErrorNotification(result.Message ?? "Failed to delete the syllabus.");
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task ApplyFileAsync(SyllabusUpdateDto model, IFormFile? file)
    {
        // FileUrl comes from the upload, not the form, so drop the binder's implicit "required" error.
        ModelState.Remove(nameof(model.FileUrl));

        var url = await UploadFileAsync(_mediaService, file, "syllabus", nameof(model.FileUrl));
        if (url is not null)
        {
            model.FileUrl = url;
            model.FileType = Path.GetExtension(file!.FileName).TrimStart('.').ToUpperInvariant();
        }

        if (string.IsNullOrWhiteSpace(model.FileUrl))
            ModelState.AddModelError(nameof(model.FileUrl), "Please upload the syllabus file (PDF, Word or image).");
    }
}
