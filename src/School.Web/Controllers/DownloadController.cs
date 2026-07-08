using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Domain.Enums;
using School.Web.Models;

namespace School.Web.Controllers;

public class DownloadController : BaseController
{
    private readonly IDownloadService _downloadService;

    public DownloadController(IDownloadService downloadService)
    {
        _downloadService = downloadService;
    }

    public async Task<IActionResult> Index(DownloadCategory? category)
    {
        var result = category.HasValue
            ? await _downloadService.GetDownloadsByCategoryAsync(category.Value)
            : await _downloadService.GetAllAsync();

        if (!result.Success || result.Data is null)
            return View(new DownloadListViewModel());

        var downloads = result.Data.Select(d => new DownloadItem
        {
            Id = d.Id,
            Title = GetLocalizedContent(d.TitleEn, d.TitleBn),
            TitleBn = d.TitleBn,
            Description = GetLocalizedContent(d.DescriptionEn ?? "", d.DescriptionBn ?? ""),
            FileUrl = d.FileUrl,
            FileType = d.FileType,
            FileSize = FormatFileSize(d.FileSize),
            Category = d.DownloadCategory,
            UploadDate = d.CreatedAt
        }).ToList();

        var viewModel = new DownloadListViewModel
        {
            Downloads = downloads,
            Category = category?.ToString()
        };

        return View(viewModel);
    }

    private static string FormatFileSize(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < suffixes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {suffixes[order]}";
    }
}
