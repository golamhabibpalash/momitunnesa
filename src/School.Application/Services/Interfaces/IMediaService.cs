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

public class MediaFileInfo
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public bool IsImage { get; set; }
}

public class MediaFolderInfo
{
    public string Name { get; set; } = string.Empty;
    /// <summary>Path relative to the uploads root, using forward slashes.</summary>
    public string Path { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}

public class MediaFolderContents
{
    public string Folder { get; set; } = string.Empty;
    public List<MediaFolderInfo> Folders { get; set; } = new();
    public List<MediaFileInfo> Files { get; set; } = new();
}

public interface IMediaService
{
    /// <summary>Folders and files directly inside an uploads sub-folder ("" for the root). Returns null for invalid paths.</summary>
    Task<MediaFolderContents?> GetFolderContentsAsync(string? folder);

    /// <summary>Normalizes a folder path relative to the uploads root; returns null if it would escape the root.</summary>
    string? NormalizeFolder(string? folder);

    Task<FileUploadResult> UploadAsync(IFormFile file, string? folder = null);
    Task<bool> DeleteAsync(string filePath);
    Task<IEnumerable<string>> GetFilesInFolderAsync(string folder);
    Task<bool> CreateFolderAsync(string folderPath);
    Task<bool> RenameFileAsync(string oldPath, string newName);
    Task<bool> DeleteFolderAsync(string folderPath);
}
