using System.Text.RegularExpressions;

namespace School.Web.Helpers;

/// <summary>
/// Gallery videos are stored as either a YouTube link or an uploaded file path (/uploads/...).
/// </summary>
public static partial class VideoHelper
{
    [GeneratedRegex(@"(?:youtube\.com/(?:watch\?(?:.*&)?v=|embed/|shorts/|live/)|youtu\.be/)([A-Za-z0-9_-]{11})", RegexOptions.IgnoreCase)]
    private static partial Regex YouTubeIdPattern();

    public static string? GetYouTubeId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        var match = YouTubeIdPattern().Match(url);
        return match.Success ? match.Groups[1].Value : null;
    }

    public static bool IsUploadedFile(string? url) =>
        !string.IsNullOrEmpty(url) && url.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase);

    public static string? GetEmbedUrl(string? url) =>
        GetYouTubeId(url) is { } id ? $"https://www.youtube-nocookie.com/embed/{id}" : null;

    /// <summary>Custom thumbnail, else the YouTube preview image, else null.</summary>
    public static string? GetThumbnail(string? url, string? customThumbnail) =>
        !string.IsNullOrEmpty(customThumbnail) ? customThumbnail
        : GetYouTubeId(url) is { } id ? $"https://img.youtube.com/vi/{id}/hqdefault.jpg"
        : null;
}
