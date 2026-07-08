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

    public async Task<IActionResult> Index()
    {
        var result = await _galleryService.GetAllAlbumsAsync();

        if (!result.Success || result.Data is null)
            return View(new GalleryIndexViewModel());

        var albums = result.Data.Select(a => new GalleryAlbum
        {
            Id = a.Id,
            Title = GetLocalizedContent(a.TitleEn, a.TitleBn),
            TitleBn = a.TitleBn,
            CoverImageUrl = a.CoverImageUrl,
            PhotoCount = a.ImageCount
        }).ToList();

        var viewModel = new GalleryIndexViewModel
        {
            Albums = albums
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Album(int id)
    {
        var albumResult = await _galleryService.GetAlbumByIdAsync(id);
        var imagesResult = await _galleryService.GetImagesByAlbumIdAsync(id);

        if (!albumResult.Success || albumResult.Data is null)
            return NotFoundResponse("Album not found.");

        var album = albumResult.Data;
        var images = imagesResult.Success && imagesResult.Data is not null
            ? imagesResult.Data.Select(i => i.ImageUrl).ToList()
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
