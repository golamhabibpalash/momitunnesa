using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Web.Models;

namespace School.Web.Controllers;

public class GalleryController : BaseController
{
    private readonly IGalleryService _galleryService;

    public GalleryController(IGalleryService galleryService)
    {
        _galleryService = galleryService;
    }

    public async Task<IActionResult> Index(string? tab)
    {
        var result = await _galleryService.GetAlbumsWithImagesAsync();
        var videosResult = await _galleryService.GetAllVideosAsync();

        var videos = (videosResult.Data ?? Enumerable.Empty<School.Application.DTOs.Gallery.GalleryVideoDto>())
            .Where(v => v.IsActive)
            .OrderBy(v => v.SortOrder)
            .ThenByDescending(v => v.CreatedAt)
            .Select(v => new GalleryVideoItem
            {
                Id = v.Id,
                Title = GetLocalizedContent(v.TitleEn, string.IsNullOrWhiteSpace(v.TitleBn) ? v.TitleEn : v.TitleBn),
                Description = v.Description,
                Url = v.YouTubeUrl,
                ThumbnailUrl = v.ThumbnailUrl
            }).ToList();

        ViewBag.ActiveTab = tab == "videos" || (result.Data?.Any(a => a.IsActive) != true && videos.Any()) ? "videos" : "photos";

        if (!result.Success || result.Data is null)
            return View(new GalleryIndexViewModel { Videos = videos });

        var albums = result.Data.Where(a => a.IsActive).OrderByDescending(a => a.CreatedAt).Select(a => new GalleryAlbum
        {
            Id = a.Id,
            Title = GetLocalizedContent(a.TitleEn, a.TitleBn),
            TitleBn = a.TitleBn,
            CoverImageUrl = a.CoverImageUrl,
            PhotoCount = a.ImageCount
        }).ToList();

        var viewModel = new GalleryIndexViewModel
        {
            Albums = albums,
            Videos = videos
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Album(int id)
    {
        var albumResult = await _galleryService.GetAlbumByIdAsync(id);
        var imagesResult = await _galleryService.GetImagesByAlbumIdAsync(id);

        if (!albumResult.Success || albumResult.Data is null || !albumResult.Data.IsActive)
            return NotFoundResponse("Album not found.");

        var album = albumResult.Data;
        var images = imagesResult.Success && imagesResult.Data is not null
            ? imagesResult.Data.OrderBy(i => i.SortOrder).ThenBy(i => i.Id).Select(i => i.ImageUrl).ToList()
            : new List<string>();

        var viewModel = new GalleryAlbumViewModel
        {
            Id = album.Id,
            Title = GetLocalizedContent(album.TitleEn, album.TitleBn),
            TitleBn = album.TitleBn,
            Description = GetLocalizedContent(album.DescriptionEn ?? "", album.DescriptionBn ?? ""),
            DescriptionBn = album.DescriptionBn,
            Images = images
        };

        return View(viewModel);
    }
}
