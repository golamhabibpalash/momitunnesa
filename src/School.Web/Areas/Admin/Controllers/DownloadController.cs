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
    private readonly IMediaService _mediaService;

    public DownloadController(
        IDownloadService downloadService,
        IMediaService mediaService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _downloadService = downloadService;
        _mediaService = mediaService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _downloadService.GetAllAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load downloads.");
            return View(new List<DownloadDto>());
        }

        return View(result.Data.OrderByDescending(d => d.CreatedAt).ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new DownloadCreateDto { DownloadCategory = DownloadCategory.Forms });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DownloadCreateDto model, IFormFile? File)
    {
        await ApplyFileAsync(model, File);

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
        ViewBag.DownloadId = id;
        return View("Create", new DownloadCreateDto
        {
            TitleEn = download.TitleEn,
            TitleBn = download.TitleBn,
            DescriptionEn = download.DescriptionEn,
            DescriptionBn = download.DescriptionBn,
            FileUrl = download.FileUrl,
            FileType = download.FileType,
            FileSize = download.FileSize,
            DownloadCategory = Enum.TryParse<DownloadCategory>(download.DownloadCategory, out var cat) ? cat : DownloadCategory.Forms,
            IsActive = download.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DownloadCreateDto model, IFormFile? File)
    {
        var previousFile = model.FileUrl;
        await ApplyFileAsync(model, File);

        if (!ModelState.IsValid)
        {
            ViewBag.DownloadId = id;
            return View("Create", model);
        }

        var result = await _downloadService.UpdateAsync(new DownloadUpdateDto
        {
            Id = id,
            TitleEn = model.TitleEn,
            TitleBn = model.TitleBn,
            DescriptionEn = model.DescriptionEn,
            DescriptionBn = model.DescriptionBn,
            FileUrl = model.FileUrl,
            FileType = model.FileType,
            FileSize = model.FileSize,
            DownloadCategory = model.DownloadCategory,
            IsActive = model.IsActive
        });

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update download.");
            ViewBag.DownloadId = id;
            return View("Create", model);
        }

        if (previousFile != model.FileUrl)
            await DeleteUploadAsync(_mediaService, previousFile);

        SetSuccessNotification("Download updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = (await _downloadService.GetByIdAsync(id)).Data;
        var result = await _downloadService.DeleteAsync(id);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to delete download.");
        }
        else
        {
            await DeleteUploadAsync(_mediaService, item?.FileUrl);
            SetSuccessNotification("Download deleted successfully.");
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task ApplyFileAsync(DownloadCreateDto model, IFormFile? file)
    {
        var url = await UploadFileAsync(_mediaService, file, "downloads", nameof(model.FileUrl));
        if (url is not null)
        {
            model.FileUrl = url;
            model.FileType = Path.GetExtension(file!.FileName).TrimStart('.').ToUpperInvariant();
            model.FileSize = file.Length;
        }

        // FileUrl is [Required]; replace the generic binder message with a clear one.
        if (string.IsNullOrWhiteSpace(model.FileUrl))
        {
            ModelState.Remove(nameof(model.FileUrl));
            ModelState.AddModelError(nameof(model.FileUrl), "Please choose the file to upload.");
        }
        else
        {
            ModelState.Remove(nameof(model.FileUrl));
        }
    }
}
