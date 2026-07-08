using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Web.Models;

namespace School.Web.Controllers;

public class SearchController : BaseController
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<IActionResult> Index(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return View(new SearchViewModel
            {
                Query = query,
                Results = new List<SearchResult>(),
                TotalResults = 0
            });
        }

        var result = await _searchService.SearchAsync(query);

        var searchResults = result.Success && result.Data is not null
            ? result.Data.Select(s => new SearchResult
            {
                Title = s.Title,
                Description = s.Description,
                Url = s.Url,
                Type = s.Type
            }).ToList()
            : new List<SearchResult>();

        var viewModel = new SearchViewModel
        {
            Query = query,
            Results = searchResults,
            TotalResults = searchResults.Count,
            CurrentPage = 1
        };

        return View(viewModel);
    }
}
