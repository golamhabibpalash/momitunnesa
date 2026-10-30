using Microsoft.AspNetCore.Http;
using School.Application.Services.Interfaces;

namespace School.Application.Services.Implementations;

public class MediaService : IMediaService
{
    private readonly string _uploadsFolder;
    private readonly long _maxFileSize = 10 * 1024 * 1024;
    private readonly long _maxVideoFileSize = 100 * 1024 * 1024;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".ico", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".mp4", ".webm", ".ogg", ".mov" };
    private readonly string[] _videoExtensions = { ".mp4", ".webm", ".ogg", ".mov" };
    private readonly string[] _imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".ico" };

    public MediaService()
    {
        _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(_uploadsFolder))
            Directory.CreateDirectory(_uploadsFolder);
    }

    public async Task<FileUploadResult> UploadAsync(IFormFile file, string? folder = null)
    {
        try
        {
            if (file is null || file.Length == 0)
                return new FileUploadResult { Success = false, Error = "No file provided." };

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return new FileUploadResult { Success = false, Error = $"File type '{extension}' is not allowed." };

            var sizeLimit = _videoExtensions.Contains(extension) ? _maxVideoFileSize : _maxFileSize;
            if (file.Length > sizeLimit)
                return new FileUploadResult { Success = false, Error = $"File size exceeds the maximum limit of {sizeLimit / 1024 / 1024}MB." };

            folder = NormalizeFolder(folder);
            if (folder is null)
                return new FileUploadResult { Success = false, Error = "Invalid folder." };

            var uploadPath = _uploadsFolder;
            if (!string.IsNullOrEmpty(folder))
            {
                uploadPath = Path.Combine(_uploadsFolder, folder);
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadPath, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = string.IsNullOrEmpty(folder)
                ? $"/uploads/{fileName}"
                : $"/uploads/{folder}/{fileName}";

            return new FileUploadResult
            {
                Success = true,
                Url = relativePath,
                FileName = fileName,
                FileSize = file.Length
            };
        }
        catch (Exception ex)
        {
            return new FileUploadResult { Success = false, Error = ex.Message };
        }
    }

    public Task<bool> DeleteAsync(string filePath)
    {
        try
        {
            var fullPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/')));
            if (!fullPath.StartsWith(Path.GetFullPath(_uploadsFolder) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public string? NormalizeFolder(string? folder)
    {
        var normalized = (folder ?? string.Empty).Replace('\\', '/').Trim('/', ' ');
        if (normalized.Length == 0)
            return string.Empty;

        var segments = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Any(s => s == "." || s == ".." || s.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0))
            return null;

        var fullPath = Path.GetFullPath(Path.Combine(_uploadsFolder, Path.Combine(segments)));
        var root = Path.GetFullPath(_uploadsFolder);
        return fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase) ? string.Join('/', segments) : null;
    }

    public Task<MediaFolderContents?> GetFolderContentsAsync(string? folder)
    {
        var relative = NormalizeFolder(folder);
        if (relative is null)
            return Task.FromResult<MediaFolderContents?>(null);

        var fullPath = relative.Length == 0 ? _uploadsFolder : Path.Combine(_uploadsFolder, relative);
        if (!Directory.Exists(fullPath))
            return Task.FromResult<MediaFolderContents?>(null);

        var prefix = relative.Length == 0 ? "" : relative + "/";
        var contents = new MediaFolderContents
        {
            Folder = relative,
            Folders = new DirectoryInfo(fullPath).GetDirectories()
                .OrderBy(d => d.Name)
                .Select(d => new MediaFolderInfo
                {
                    Name = d.Name,
                    Path = prefix + d.Name,
                    ItemCount = d.EnumerateFileSystemInfos().Count()
                }).ToList(),
            Files = new DirectoryInfo(fullPath).GetFiles()
                .OrderByDescending(f => f.LastWriteTimeUtc)
                .Select(f => new MediaFileInfo
                {
                    Name = f.Name,
                    Url = $"/uploads/{prefix}{f.Name}",
                    Size = f.Length,
                    LastModified = f.LastWriteTimeUtc,
                    IsImage = _imageExtensions.Contains(f.Extension.ToLowerInvariant())
                }).ToList()
        };

        return Task.FromResult<MediaFolderContents?>(contents);
    }

    public Task<IEnumerable<string>> GetFilesInFolderAsync(string folder)
    {
        var relative = NormalizeFolder(folder);
        var folderPath = relative is null ? null : Path.Combine(_uploadsFolder, relative);
        if (folderPath is null || !Directory.Exists(folderPath))
            return Task.FromResult(Enumerable.Empty<string>());

        var prefix = relative!.Length == 0 ? "" : relative + "/";
        var files = Directory.GetFiles(folderPath)
            .Select(f => $"/uploads/{prefix}{Path.GetFileName(f)}");
        return Task.FromResult(files);
    }

    public Task<bool> CreateFolderAsync(string folderPath)
    {
        try
        {
            var relative = NormalizeFolder(folderPath);
            if (string.IsNullOrEmpty(relative))
                return Task.FromResult(false);

            var fullPath = Path.Combine(_uploadsFolder, relative);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> RenameFileAsync(string oldPath, string newName)
    {
        try
        {
            var fullOldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldPath.TrimStart('/'));
            if (!File.Exists(fullOldPath))
                return Task.FromResult(false);

            var dir = Path.GetDirectoryName(fullOldPath);
            var extension = Path.GetExtension(fullOldPath);
            var fullNewPath = Path.Combine(dir!, $"{newName}{extension}");

            File.Move(fullOldPath, fullNewPath);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> DeleteFolderAsync(string folderPath)
    {
        try
        {
            var relative = NormalizeFolder(folderPath);
            if (string.IsNullOrEmpty(relative))
                return Task.FromResult(false);

            var fullPath = Path.Combine(_uploadsFolder, relative);
            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}
