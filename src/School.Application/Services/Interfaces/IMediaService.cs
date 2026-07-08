using Microsoft.AspNetCore.Http;

namespace School.Application.Services.Interfaces;

public class FileUploadResult
{
    public bool Success { get; set; }
    public string? Url { get; set; }
    public string? FileName { get; set; }
    public long FileSize { get; set; }
    public string? Error { get; set; }
}

public interface IMediaService
{
    Task<FileUploadResult> UploadAsync(IFormFile file, string? folder = null);
    Task<bool> DeleteAsync(string filePath);
    Task<IEnumerable<string>> GetFilesInFolderAsync(string folder);
    Task<bool> CreateFolderAsync(string folderPath);
    Task<bool> RenameFileAsync(string oldPath, string newName);
    Task<bool> DeleteFolderAsync(string folderPath);
}
