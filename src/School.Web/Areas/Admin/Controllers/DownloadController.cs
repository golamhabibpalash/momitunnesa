using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Download;
using School.Application.Services.Interfaces;
using School.Domain.Enums;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class DownloadController : BaseAdminController
{
    private readonly IDownloadService _downloadService;

    public DownloadController(
        IDownloadService downloadService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _downloadService = downloadService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _downloadService.GetAllAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load downloads.");
            return View(new List<DownloadDto>());
        }

        return View(result.Data.OrderByDescending(d => d.CreatedAt));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new DownloadCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DownloadCreateDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _downloadService.CreateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create download.");
            return View(model);
        }

        SetSuccessNotification("Download created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _downloadService.GetByIdAsync(id);

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification("Download not found.");
            return RedirectToAction(nameof(Index));
        }

        var download = result.Data;
        var dto = new DownloadCreateDto
        {
            TitleEn = download.TitleEn,
            TitleBn = download.TitleBn,
            DescriptionEn = download.DescriptionEn,
            DescriptionBn = download.DescriptionBn,
            FileUrl = download.FileUrl,
            FileType = download.FileType,
            DownloadCategory = Enum.TryParse<DownloadCategory>(download.DownloadCategory, out var cat) ? cat : DownloadCategory.Circular,
            FileSize = download.FileSize,
            IsActive = download.IsActive
        };

        ViewBag.DownloadId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DownloadCreateDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.DownloadId = id;
            return View(model);
        }

        var updateDto = new DownloadUpdateDto
        {
            Id = id,
            TitleEn = model.TitleEn,
            TitleBn = model.TitleBn,
            DescriptionEn = model.DescriptionEn,
            DescriptionBn = model.DescriptionBn,
            FileUrl = model.FileUrl,
            FileType = model.FileType,
            DownloadCategory = model.DownloadCategory,
            FileSize = model.FileSize,
            IsActive = model.IsActive
        };

        var result = await _downloadService.UpdateAsync(updateDto);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update download.");
            ViewBag.DownloadId = id;
            return View(model);
        }

        SetSuccessNotification("Download updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _downloadService.DeleteAsync(id);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to delete download.");
        }
        else
        {
            SetSuccessNotification("Download deleted successfully.");
        }

        return RedirectToAction(nameof(Index));
    }
}
