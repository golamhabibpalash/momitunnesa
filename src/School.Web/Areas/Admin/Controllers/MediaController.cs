using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class MediaController : BaseAdminController
{
    private const long UploadRequestLimit = 300L * 1024 * 1024;

    private readonly IMediaService _mediaService;

    public MediaController(
        IMediaService mediaService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _mediaService = mediaService;
    }

    public async Task<IActionResult> Index(string? folder = "")
    {
        var contents = await _mediaService.GetFolderContentsAsync(folder);
        if (contents is null)
        {
            SetErrorNotification("Folder not found.");
            contents = await _mediaService.GetFolderContentsAsync("") ?? new MediaFolderContents();
        }

        return View(contents);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(UploadRequestLimit)]
    [RequestFormLimits(MultipartBodyLengthLimit = UploadRequestLimit)]
    public async Task<IActionResult> Upload(List<IFormFile> files, string? folder)
    {
        var selected = files.Where(f => f.Length > 0).ToList();
        if (selected.Count == 0)
        {
            SetErrorNotification("Please select at least one file to upload.");
            return RedirectToAction(nameof(Index), new { folder });
        }

        var failures = new List<string>();
        foreach (var file in selected)
        {
            var result = await _mediaService.UploadAsync(file, folder);
            if (!result.Success)
                failures.Add($"{file.FileName}: {result.Error}");
        }

        var uploaded = selected.Count - failures.Count;
        if (uploaded > 0)
            SetSuccessNotification($"{uploaded} file(s) uploaded successfully.");
        if (failures.Count > 0)
            SetErrorNotification("Some files failed: " + string.Join("; ", failures.Take(5)));

        return RedirectToAction(nameof(Index), new { folder });
    }

    /// <summary>Image upload endpoint for the TinyMCE editor; returns { location } as TinyMCE expects.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditorUpload(IFormFile file)
    {
        var extension = Path.GetExtension(file?.FileName ?? "").ToLowerInvariant();
        if (file is null || extension is not (".jpg" or ".jpeg" or ".png" or ".gif" or ".webp"))
            return BadRequest(new { error = "Please upload a JPG, PNG, GIF or WEBP image." });

        var result = await _mediaService.UploadAsync(file, "editor");
        if (!result.Success)
            return BadRequest(new { error = result.Error });

        return Json(new { location = result.Url });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFolder(string? folderName, string? parentFolder)
    {
        var name = (folderName ?? "").Trim();
        if (name.Length == 0 || name.Contains('/') || name.Contains('\\') || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || name.StartsWith('.'))
        {
            SetErrorNotification("Please enter a valid folder name (letters, numbers, spaces, - and _).");
            return RedirectToAction(nameof(Index), new { folder = parentFolder });
        }

        var parent = _mediaService.NormalizeFolder(parentFolder);
        if (parent is null)
        {
            SetErrorNotification("Invalid parent folder.");
            return RedirectToAction(nameof(Index));
        }

        var path = parent.Length == 0 ? name : $"{parent}/{name}";
        var existing = await _mediaService.GetFolderContentsAsync(path);
        if (existing is not null)
        {
            SetErrorNotification($"A folder named \"{name}\" already exists here.");
            return RedirectToAction(nameof(Index), new { folder = parent });
        }

        if (!await _mediaService.CreateFolderAsync(path))
        {
            SetErrorNotification("Failed to create folder.");
            return RedirectToAction(nameof(Index), new { folder = parent });
        }

        SetSuccessNotification($"Folder \"{name}\" created.");
        return RedirectToAction(nameof(Index), new { folder = path });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFolder(string folder)
    {
        var contents = await _mediaService.GetFolderContentsAsync(folder);
        var parent = folder.Contains('/') ? folder[..folder.LastIndexOf('/')] : "";

        if (contents is null || contents.Folder.Length == 0)
        {
            SetErrorNotification("Folder not found.");
        }
        else if (contents.Files.Count > 0 || contents.Folders.Count > 0)
        {
            SetErrorNotification("Only empty folders can be deleted. Delete or move the files inside first.");
            return RedirectToAction(nameof(Index), new { folder });
        }
        else if (await _mediaService.DeleteFolderAsync(contents.Folder))
        {
            SetSuccessNotification("Folder deleted.");
        }
        else
        {
            SetErrorNotification("Failed to delete folder.");
        }

        return RedirectToAction(nameof(Index), new { folder = parent });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string path, string? folder)
    {
        if (string.IsNullOrWhiteSpace(path) || !path.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
        {
            SetErrorNotification("File path is required.");
            return RedirectToAction(nameof(Index), new { folder });
        }

        if (await _mediaService.DeleteAsync(path))
            SetSuccessNotification("File deleted successfully.");
        else
            SetErrorNotification("Failed to delete file.");

        return RedirectToAction(nameof(Index), new { folder });
    }
}
