using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Gallery;
using School.Application.Services.Interfaces;
using School.Web.Helpers;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class GalleryController : BaseAdminController
{
    // Bulk photo uploads and video files are larger than Kestrel's 30 MB default request limit.
    private const long BulkUploadLimit = 300L * 1024 * 1024;
    private const long VideoUploadLimit = 110L * 1024 * 1024;

    private readonly IGalleryService _galleryService;
    private readonly IMediaService _mediaService;

    public GalleryController(
        IGalleryService galleryService,
        IMediaService mediaService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _galleryService = galleryService;
        _mediaService = mediaService;
    }

    // ---------- Albums ----------

    public async Task<IActionResult> Albums()
    {
        var result = await _galleryService.GetAlbumsWithImagesAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load albums.");
            return View(new List<GalleryAlbumDto>());
        }

        return View(result.Data.OrderByDescending(a => a.CreatedAt).ToList());
    }

    [HttpGet]
    public IActionResult CreateAlbum()
    {
        return View("AlbumForm", new GalleryAlbumCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAlbum(GalleryAlbumCreateDto model, IFormFile? CoverImage)
    {
        var uploadedUrl = await UploadFileAsync(_mediaService, CoverImage, "gallery/covers", nameof(model.CoverImageUrl));
        if (uploadedUrl is not null)
            model.CoverImageUrl = uploadedUrl;

        if (!ModelState.IsValid)
            return View("AlbumForm", model);

        var result = await _galleryService.CreateAlbumAsync(model);

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to create album.");
            return View("AlbumForm", model);
        }

        SetSuccessNotification("Album created. Now add photos to it.");
        return RedirectToAction(nameof(AlbumImages), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> EditAlbum(int id)
    {
        var result = await _galleryService.GetAlbumByIdAsync(id);

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification("Album not found.");
            return RedirectToAction(nameof(Albums));
        }

        var album = result.Data;
        ViewBag.AlbumId = id;
        return View("AlbumForm", new GalleryAlbumCreateDto
        {
            TitleEn = album.TitleEn,
            TitleBn = album.TitleBn,
            DescriptionEn = album.DescriptionEn,
            DescriptionBn = album.DescriptionBn,
            CoverImageUrl = album.CoverImageUrl,
            IsActive = album.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAlbum(int id, GalleryAlbumCreateDto model, IFormFile? CoverImage)
    {
        var previousCover = model.CoverImageUrl;
        var uploadedUrl = await UploadFileAsync(_mediaService, CoverImage, "gallery/covers", nameof(model.CoverImageUrl));
        if (uploadedUrl is not null)
            model.CoverImageUrl = uploadedUrl;

        if (!ModelState.IsValid)
        {
            ViewBag.AlbumId = id;
            return View("AlbumForm", model);
        }

        var result = await _galleryService.UpdateAlbumAsync(ToUpdateDto(id, model));

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update album.");
            ViewBag.AlbumId = id;
            return View("AlbumForm", model);
        }

        // Only delete an uploaded cover file, never a photo that is still part of the album.
        if (uploadedUrl is not null && previousCover?.StartsWith("/uploads/gallery/covers/", StringComparison.OrdinalIgnoreCase) == true)
            await DeleteUploadAsync(_mediaService, previousCover);

        SetSuccessNotification("Album updated successfully.");
        return RedirectToAction(nameof(AlbumImages), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAlbum(int id)
    {
        var result = await _galleryService.DeleteAlbumAsync(id);

        if (!result.Success)
            SetErrorNotification(result.Message ?? "Failed to delete album.");
        else
            SetSuccessNotification("Album deleted successfully.");

        return RedirectToAction(nameof(Albums));
    }

    // ---------- Album images ----------

    public async Task<IActionResult> AlbumImages(int id)
    {
        var albumResult = await _galleryService.GetAlbumByIdAsync(id);

        if (!albumResult.Success || albumResult.Data is null)
        {
            SetErrorNotification("Album not found.");
            return RedirectToAction(nameof(Albums));
        }

        var imagesResult = await _galleryService.GetImagesByAlbumIdAsync(id);
        ViewBag.Album = albumResult.Data;
        var images = imagesResult.Success && imagesResult.Data is not null
            ? imagesResult.Data.OrderBy(i => i.SortOrder).ThenBy(i => i.Id).ToList()
            : new List<GalleryImageDto>();

        return View(images);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(BulkUploadLimit)]
    [RequestFormLimits(MultipartBodyLengthLimit = BulkUploadLimit)]
    public async Task<IActionResult> UploadImages(int albumId, List<IFormFile> files)
    {
        var albumResult = await _galleryService.GetAlbumByIdAsync(albumId);
        if (!albumResult.Success || albumResult.Data is null)
        {
            SetErrorNotification("Album not found.");
            return RedirectToAction(nameof(Albums));
        }

        var images = files.Where(f => f.Length > 0).ToList();
        if (images.Count == 0)
        {
            SetErrorNotification("Please select at least one image to upload.");
            return RedirectToAction(nameof(AlbumImages), new { id = albumId });
        }

        var existing = await _galleryService.GetImagesByAlbumIdAsync(albumId);
        var nextSortOrder = (existing.Data?.Select(i => i.SortOrder).DefaultIfEmpty(0).Max() ?? 0) + 1;
        var uploaded = 0;
        string? firstUrl = null;
        var failures = new List<string>();

        foreach (var file in images)
        {
            if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                failures.Add($"{file.FileName}: not an image");
                continue;
            }

            var uploadResult = await _mediaService.UploadAsync(file, $"gallery/album-{albumId}");
            if (!uploadResult.Success || uploadResult.Url is null)
            {
                failures.Add($"{file.FileName}: {uploadResult.Error}");
                continue;
            }

            var saveResult = await _galleryService.CreateImageAsync(new GalleryImageCreateDto
            {
                GalleryAlbumId = albumId,
                ImageUrl = uploadResult.Url,
                Title = Path.GetFileNameWithoutExtension(file.FileName),
                SortOrder = nextSortOrder++
            });

            if (!saveResult.Success)
            {
                await _mediaService.DeleteAsync(uploadResult.Url);
                failures.Add($"{file.FileName}: {saveResult.Message}");
                continue;
            }

            firstUrl ??= uploadResult.Url;
            uploaded++;
        }

        // Give a new album a cover automatically.
        if (firstUrl is not null && string.IsNullOrEmpty(albumResult.Data.CoverImageUrl))
            await SetCoverAsync(albumResult.Data, firstUrl);

        if (uploaded > 0)
            SetSuccessNotification($"{uploaded} image(s) uploaded successfully.");
        if (failures.Count > 0)
            SetErrorNotification($"{failures.Count} file(s) failed: " + string.Join("; ", failures.Take(5)));

        return RedirectToAction(nameof(AlbumImages), new { id = albumId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImages(int albumId, List<int> imageIds)
    {
        if (imageIds.Count == 0)
        {
            SetErrorNotification("Select at least one image to delete.");
            return RedirectToAction(nameof(AlbumImages), new { id = albumId });
        }

        var albumResult = await _galleryService.GetAlbumByIdAsync(albumId);
        var imagesResult = await _galleryService.GetImagesByAlbumIdAsync(albumId);
        var albumImages = imagesResult.Data?.ToList() ?? new();
        var deleted = 0;

        foreach (var image in albumImages.Where(i => imageIds.Contains(i.Id)))
        {
            var result = await _galleryService.DeleteImageAsync(image.Id);
            if (!result.Success)
                continue;

            deleted++;
            await DeleteUploadAsync(_mediaService, image.ImageUrl);

            if (albumResult.Data is not null && albumResult.Data.CoverImageUrl == image.ImageUrl)
            {
                var nextCover = albumImages.FirstOrDefault(i => !imageIds.Contains(i.Id))?.ImageUrl;
                await SetCoverAsync(albumResult.Data, nextCover);
                albumResult.Data.CoverImageUrl = nextCover;
            }
        }

        SetSuccessNotification($"{deleted} image(s) deleted.");
        return RedirectToAction(nameof(AlbumImages), new { id = albumId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetCover(int albumId, int imageId)
    {
        var albumResult = await _galleryService.GetAlbumByIdAsync(albumId);
        var imagesResult = await _galleryService.GetImagesByAlbumIdAsync(albumId);
        var image = imagesResult.Data?.FirstOrDefault(i => i.Id == imageId);

        if (albumResult.Data is null || image is null)
        {
            SetErrorNotification("Image not found.");
            return RedirectToAction(nameof(AlbumImages), new { id = albumId });
        }

        await SetCoverAsync(albumResult.Data, image.ImageUrl);
        SetSuccessNotification("Cover image updated.");
        return RedirectToAction(nameof(AlbumImages), new { id = albumId });
    }

    // ---------- Videos ----------

    public async Task<IActionResult> Videos()
    {
        var result = await _galleryService.GetAllVideosAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load videos.");
            return View(new List<GalleryVideoDto>());
        }

        return View(result.Data.OrderBy(v => v.SortOrder).ThenByDescending(v => v.CreatedAt).ToList());
    }

    [HttpGet]
    public IActionResult CreateVideo()
    {
        return View("VideoForm", new GalleryVideoCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(VideoUploadLimit)]
    [RequestFormLimits(MultipartBodyLengthLimit = VideoUploadLimit)]
    public async Task<IActionResult> CreateVideo(GalleryVideoCreateDto model, IFormFile? VideoFile, IFormFile? Thumbnail)
    {
        await ApplyVideoUploadsAsync(model, VideoFile, Thumbnail);

        if (!ModelState.IsValid)
            return View("VideoForm", model);

        var result = await _galleryService.CreateVideoAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create video.");
            return View("VideoForm", model);
        }

        SetSuccessNotification("Video added successfully.");
        return RedirectToAction(nameof(Videos));
    }

    [HttpGet]
    public async Task<IActionResult> EditVideo(int id)
    {
        var result = await _galleryService.GetVideoByIdAsync(id);

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification("Video not found.");
            return RedirectToAction(nameof(Videos));
        }

        var video = result.Data;
        ViewBag.VideoId = id;
        return View("VideoForm", new GalleryVideoCreateDto
        {
            TitleEn = video.TitleEn,
            TitleBn = video.TitleBn,
            Description = video.Description,
            YouTubeUrl = video.YouTubeUrl,
            ThumbnailUrl = video.ThumbnailUrl,
            IsActive = video.IsActive,
            SortOrder = video.SortOrder
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(VideoUploadLimit)]
    [RequestFormLimits(MultipartBodyLengthLimit = VideoUploadLimit)]
    public async Task<IActionResult> EditVideo(int id, GalleryVideoCreateDto model, IFormFile? VideoFile, IFormFile? Thumbnail)
    {
        var previousVideo = model.YouTubeUrl;
        var previousThumbnail = model.ThumbnailUrl;
        await ApplyVideoUploadsAsync(model, VideoFile, Thumbnail);

        if (!ModelState.IsValid)
        {
            ViewBag.VideoId = id;
            return View("VideoForm", model);
        }

        var result = await _galleryService.UpdateVideoAsync(new GalleryVideoUpdateDto
        {
            Id = id,
            TitleEn = model.TitleEn,
            TitleBn = model.TitleBn,
            Description = model.Description,
            YouTubeUrl = model.YouTubeUrl,
            ThumbnailUrl = model.ThumbnailUrl,
            IsActive = model.IsActive,
            SortOrder = model.SortOrder
        });

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update video.");
            ViewBag.VideoId = id;
            return View("VideoForm", model);
        }

        if (previousVideo != model.YouTubeUrl)
            await DeleteUploadAsync(_mediaService, previousVideo);
        if (previousThumbnail != model.ThumbnailUrl)
            await DeleteUploadAsync(_mediaService, previousThumbnail);

        SetSuccessNotification("Video updated successfully.");
        return RedirectToAction(nameof(Videos));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteVideo(int id)
    {
        var video = (await _galleryService.GetVideoByIdAsync(id)).Data;
        var result = await _galleryService.DeleteVideoAsync(id);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to delete video.");
        }
        else
        {
            await DeleteUploadAsync(_mediaService, video?.YouTubeUrl);
            await DeleteUploadAsync(_mediaService, video?.ThumbnailUrl);
            SetSuccessNotification("Video deleted successfully.");
        }

        return RedirectToAction(nameof(Videos));
    }

    // ---------- Helpers ----------

    /// <summary>
    /// A video is either a YouTube link or an uploaded file; both are stored in YouTubeUrl.
    /// The [Url] attribute rejects relative upload paths, so its error is replaced by our own checks.
    /// </summary>
    private async Task ApplyVideoUploadsAsync(GalleryVideoCreateDto model, IFormFile? videoFile, IFormFile? thumbnail)
    {
        ModelState.Remove(nameof(model.YouTubeUrl));

        if (videoFile is { Length: > 0 })
        {
            var videoUrl = await UploadFileAsync(_mediaService, videoFile, "gallery/videos", nameof(model.YouTubeUrl));
            if (videoUrl is not null)
                model.YouTubeUrl = videoUrl;
        }
        else if (!string.IsNullOrWhiteSpace(model.YouTubeUrl) && !model.YouTubeUrl.StartsWith("/uploads/")
                 && VideoHelper.GetYouTubeId(model.YouTubeUrl) is null)
        {
            ModelState.AddModelError(nameof(model.YouTubeUrl), "Enter a valid YouTube link (youtube.com/watch?v=… or youtu.be/…).");
        }

        if (string.IsNullOrWhiteSpace(model.YouTubeUrl))
            ModelState.AddModelError(nameof(model.YouTubeUrl), "Provide a YouTube link or upload a video file.");

        var thumbnailUrl = await UploadFileAsync(_mediaService, thumbnail, "gallery/videos", nameof(model.ThumbnailUrl));
        if (thumbnailUrl is not null)
            model.ThumbnailUrl = thumbnailUrl;
    }

    private async Task SetCoverAsync(GalleryAlbumDto album, string? coverUrl)
    {
        await _galleryService.UpdateAlbumAsync(new GalleryAlbumUpdateDto
        {
            Id = album.Id,
            TitleEn = album.TitleEn,
            TitleBn = album.TitleBn,
            DescriptionEn = album.DescriptionEn,
            DescriptionBn = album.DescriptionBn,
            CoverImageUrl = coverUrl,
            IsActive = album.IsActive
        });
    }

    private static GalleryAlbumUpdateDto ToUpdateDto(int id, GalleryAlbumCreateDto model) => new()
    {
        Id = id,
        TitleEn = model.TitleEn,
        TitleBn = model.TitleBn,
        DescriptionEn = model.DescriptionEn,
        DescriptionBn = model.DescriptionBn,
        CoverImageUrl = model.CoverImageUrl,
        IsActive = model.IsActive
    };
}
