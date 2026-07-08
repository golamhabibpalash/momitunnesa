using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Domain.Enums;

namespace School.Web.Controllers;

public class PublicationController : BaseController
{
    private readonly IDownloadService _downloadService;

    public PublicationController(IDownloadService downloadService)
    {
        _downloadService = downloadService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _downloadService.GetAllAsync();

        if (!result.Success || result.Data is null)
            return View(new List<School.Application.DTOs.Download.DownloadDto>());

        var publications = result.Data.Where(d => d.DownloadCategory == DownloadCategory.Reports.ToString());

        return View(publications);
    }
}
