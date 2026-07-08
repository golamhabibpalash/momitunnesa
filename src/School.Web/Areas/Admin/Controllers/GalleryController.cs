using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Gallery;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class GalleryController : BaseAdminController
{
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

    public async Task<IActionResult> Albums()
    {
        var result = await _galleryService.GetAllAlbumsAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load albums.");
            return View(new List<GalleryAlbumDto>());
        }

        return View(result.Data.OrderByDescending(a => a.CreatedAt));
    }

    [HttpGet]
    public IActionResult CreateAlbum()
    {
        return View(new GalleryAlbumCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAlbum(GalleryAlbumCreateDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _galleryService.CreateAlbumAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create album.");
            return View(model);
        }

        SetSuccessNotification("Album created successfully.");
        return RedirectToAction(nameof(Albums));
    }

    public async Task<IActionResult> AlbumImages(int id)
    {
        var albumResult = await _galleryService.GetAlbumByIdAsync(id);
        var imagesResult = await _galleryService.GetImagesByAlbumIdAsync(id);

        if (!albumResult.Success || albumResult.Data is null)
        {
            SetErrorNotification("Album not found.");
            return RedirectToAction(nameof(Albums));
        }

        ViewBag.Album = albumResult.Data;
        ViewBag.Images = imagesResult.Success && imagesResult.Data is not null ? imagesResult.Data : new List<GalleryImageDto>();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadImage(int albumId, IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            SetErrorNotification("Please select a file to upload.");
            return RedirectToAction(nameof(AlbumImages), new { id = albumId });
        }

        var uploadResult = await _mediaService.UploadAsync(file, $"gallery/album-{albumId}");

        if (!uploadResult.Success)
        {
            SetErrorNotification(uploadResult.Error ?? "Failed to upload image.");
            return RedirectToAction(nameof(AlbumImages), new { id = albumId });
        }

        var imageDto = new GalleryImageCreateDto
        {
            GalleryAlbumId = albumId,
            ImageUrl = uploadResult.Url ?? "",
            Title = file.FileName
        };

        var result = await _galleryService.CreateImageAsync(imageDto);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to save image.");
            return RedirectToAction(nameof(AlbumImages), new { id = albumId });
        }

        SetSuccessNotification("Image uploaded successfully.");
        return RedirectToAction(nameof(AlbumImages), new { id = albumId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int id)
    {
        var result = await _galleryService.DeleteImageAsync(id);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to delete image.");
        }
        else
        {
            SetSuccessNotification("Image deleted successfully.");
        }

        return RedirectToAction(nameof(Albums));
    }

    public async Task<IActionResult> Videos()
    {
        var result = await _galleryService.GetAllVideosAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load videos.");
            return View(new List<GalleryVideoDto>());
        }

        return View(result.Data.OrderByDescending(v => v.CreatedAt));
    }

    [HttpGet]
    public IActionResult CreateVideo()
    {
        return View(new GalleryVideoCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateVideo(GalleryVideoCreateDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _galleryService.CreateVideoAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create video.");
            return View(model);
        }

        SetSuccessNotification("Video created successfully.");
        return RedirectToAction(nameof(Videos));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteVideo(int id)
    {
        var result = await _galleryService.DeleteVideoAsync(id);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to delete video.");
        }
        else
        {
            SetSuccessNotification("Video deleted successfully.");
        }

        return RedirectToAction(nameof(Videos));
    }
}
