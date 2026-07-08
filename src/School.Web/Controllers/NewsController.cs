using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Web.Models;

namespace School.Web.Controllers;

public class NewsController : BaseController
{
    private readonly INewsService _newsService;
    private const int PageSize = 9;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _newsService.GetActiveNewsAsync();

        if (!result.Success || result.Data is null)
            return View(new NewsListViewModel());

        var newsItems = result.Data.Select(n => new NewsItem
        {
            Id = n.Id,
            Title = GetLocalizedContent(n.TitleEn, n.TitleBn),
            TitleBn = n.TitleBn,
            Description = GetLocalizedContent(n.ContentEn ?? "", n.ContentBn ?? ""),
            ImageUrl = n.ImageUrl,
            Category = n.Category,
            PublishDate = n.PublishDate
        }).ToList();

        var totalItems = newsItems.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
        var pagedItems = newsItems.Skip((page - 1) * PageSize).Take(PageSize).ToList();

        var viewModel = new NewsListViewModel
        {
            NewsItems = pagedItems,
            CurrentPage = page,
            TotalPages = totalPages
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _newsService.GetByIdAsync(id);

        if (!result.Success || result.Data is null)
            return NotFoundResponse("News article not found.");

        var news = result.Data;
        var viewModel = new NewsViewModel
        {
            Id = news.Id,
            Title = GetLocalizedContent(news.TitleEn, news.TitleBn),
            TitleBn = news.TitleBn,
            Content = GetLocalizedContent(news.ContentEn ?? "", news.ContentBn ?? ""),
            ContentBn = news.ContentBn,
            ImageUrl = news.ImageUrl,
            Category = news.Category,
            PublishDate = news.PublishDate
        };

        return View(viewModel);
    }
}
