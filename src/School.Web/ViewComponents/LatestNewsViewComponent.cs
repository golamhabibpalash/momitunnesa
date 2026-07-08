using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;

namespace School.Web.ViewComponents;

public class LatestNewsViewComponent : ViewComponent
{
    private readonly INewsService _newsService;

    public LatestNewsViewComponent(INewsService newsService)
    {
        _newsService = newsService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int count = 3)
    {
        var result = await _newsService.GetActiveNewsAsync();
        var news = (result.Data ?? Enumerable.Empty<School.Application.DTOs.News.NewsDto>())
            .Take(count);
        return View(news);
    }
}
