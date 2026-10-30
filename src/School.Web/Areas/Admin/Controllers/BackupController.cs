using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class BackupController : BaseAdminController
{
    private readonly IApplicationDataService _dataService;

    public BackupController(
        IApplicationDataService dataService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _dataService = dataService;
    }

    public IActionResult Index()
    {
        var uploads = UploadsFolder();
        var files = Directory.Exists(uploads) ? new DirectoryInfo(uploads).GetFiles("*", SearchOption.AllDirectories) : Array.Empty<FileInfo>();
        ViewBag.UploadCount = files.Length;
        ViewBag.UploadSize = files.Sum(f => f.Length);
        return View();
    }

    /// <summary>
    /// Streams a ZIP with data.json (all website content) and, optionally, the uploads folder.
    /// Built in a temp file so a large uploads folder does not sit in memory.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Download(bool includeUploads = true)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"school-backup-{Guid.NewGuid():N}.zip");

        try
        {
            await using (var zipStream = new FileStream(tempPath, FileMode.Create))
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
            {
                var dataEntry = archive.CreateEntry("data.json", CompressionLevel.Optimal);
                await using (var entryStream = dataEntry.Open())
                    await _dataService.ExportContentAsJsonAsync(entryStream);

                var uploads = UploadsFolder();
                if (includeUploads && Directory.Exists(uploads))
                {
                    foreach (var file in Directory.EnumerateFiles(uploads, "*", SearchOption.AllDirectories))
                    {
                        var relative = Path.GetRelativePath(uploads, file).Replace('\\', '/');
                        archive.CreateEntryFromFile(file, $"uploads/{relative}", CompressionLevel.Fastest);
                    }
                }
            }

            _logger.LogInformation("Backup downloaded by {User} (uploads included: {IncludeUploads}).", User.Identity?.Name, includeUploads);
            var stream = new FileStream(tempPath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose);
            return File(stream, "application/zip", $"school-backup-{DateTime.Now:yyyyMMdd-HHmm}.zip");
        }
        catch (Exception ex)
        {
            if (System.IO.File.Exists(tempPath))
                System.IO.File.Delete(tempPath);

            _logger.LogError(ex, "Backup failed.");
            SetErrorNotification("Backup failed: " + ex.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    // MediaService stores uploads under the current directory's wwwroot (see AGENTS.md).
    private static string UploadsFolder() => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
}
