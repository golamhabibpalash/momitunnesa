using Microsoft.AspNetCore.Http;
using School.Application.Services.Interfaces;

namespace School.Application.Services.Implementations;

public class MediaService : IMediaService
{
    private readonly string _uploadsFolder;
    private readonly long _maxFileSize = 10 * 1024 * 1024;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".ico", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt" };
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

            if (file.Length > _maxFileSize)
                return new FileUploadResult { Success = false, Error = $"File size exceeds the maximum limit of {_maxFileSize / 1024 / 1024}MB." };

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return new FileUploadResult { Success = false, Error = $"File type '{extension}' is not allowed." };

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
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));
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

    public Task<IEnumerable<string>> GetFilesInFolderAsync(string folder)
    {
        var folderPath = Path.Combine(_uploadsFolder, folder);
        if (!Directory.Exists(folderPath))
            return Task.FromResult(Enumerable.Empty<string>());

        var files = Directory.GetFiles(folderPath)
            .Select(f => $"/uploads/{folder}/{Path.GetFileName(f)}");
        return Task.FromResult(files);
    }

    public Task<bool> CreateFolderAsync(string folderPath)
    {
        try
        {
            var fullPath = Path.Combine(_uploadsFolder, folderPath);
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
            var fullPath = Path.Combine(_uploadsFolder, folderPath);
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
