using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class MediaController : BaseAdminController
{
    private readonly IMediaService _mediaService;

    public MediaController(
        IMediaService mediaService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _mediaService = mediaService;
    }

    public async Task<IActionResult> Index(string folder = "")
    {
        var files = await _mediaService.GetFilesInFolderAsync(folder);
        ViewBag.CurrentFolder = folder;
        ViewBag.Files = files ?? Enumerable.Empty<string>();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile file, string folder)
    {
        if (file is null || file.Length == 0)
        {
            SetErrorNotification("Please select a file to upload.");
            return RedirectToAction(nameof(Index), new { folder });
        }

        var result = await _mediaService.UploadAsync(file, folder);

        if (!result.Success)
        {
            SetErrorNotification(result.Error ?? "Failed to upload file.");
        }
        else
        {
            SetSuccessNotification("File uploaded successfully.");
        }

        return RedirectToAction(nameof(Index), new { folder });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFolder(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            SetErrorNotification("Folder name is required.");
            return RedirectToAction(nameof(Index));
        }

        var result = await _mediaService.CreateFolderAsync(name);

        if (!result)
        {
            SetErrorNotification("Failed to create folder.");
        }
        else
        {
            SetSuccessNotification("Folder created successfully.");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            SetErrorNotification("File path is required.");
            return RedirectToAction(nameof(Index));
        }

        var result = await _mediaService.DeleteAsync(path);

        if (!result)
        {
            SetErrorNotification("Failed to delete file.");
        }
        else
        {
            SetSuccessNotification("File deleted successfully.");
        }

        return RedirectToAction(nameof(Index));
    }
}
