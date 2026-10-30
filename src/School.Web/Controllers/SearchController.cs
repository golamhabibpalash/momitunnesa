using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Web.Models;

namespace School.Web.Controllers;

public class SearchController : BaseController
{
    private static readonly Dictionary<string, string> TypeLabelsBn = new()
    {
        ["Page"] = "পাতা",
        ["Notice"] = "নোটিশ",
        ["News"] = "খবর",
        ["Event"] = "ইভেন্ট",
        ["Teacher"] = "শিক্ষক",
        ["Download"] = "ডাউনলোড",
        ["Gallery"] = "গ্যালারি"
    };

    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    // The header search box submits "q"; "query" is kept for older links.
    public async Task<IActionResult> Index(string? q, string? query)
    {
        var term = (q ?? query)?.Trim();

        if (string.IsNullOrWhiteSpace(term))
        {
            return View(new SearchViewModel
            {
                Query = term,
                Results = new List<SearchResult>(),
                TotalResults = 0
            });
        }

        var result = await _searchService.SearchAsync(term);

        var searchResults = result.Success && result.Data is not null
            ? result.Data.Select(s => new SearchResult
            {
                Title = GetLocalizedContent(s.Title, string.IsNullOrWhiteSpace(s.TitleBn) ? s.Title : s.TitleBn),
                Description = GetLocalizedContent(s.Description ?? "", string.IsNullOrWhiteSpace(s.DescriptionBn) ? s.Description ?? "" : s.DescriptionBn),
                Url = s.Url,
                Type = s.Type,
                TypeBn = TypeLabelsBn.GetValueOrDefault(s.Type),
                Date = s.Date?.ToString("dd MMM yyyy")
            }).ToList()
            : new List<SearchResult>();

        var viewModel = new SearchViewModel
        {
            Query = term,
            Results = searchResults,
            TotalResults = searchResults.Count,
            CurrentPage = 1
        };

        return View(viewModel);
    }
}
