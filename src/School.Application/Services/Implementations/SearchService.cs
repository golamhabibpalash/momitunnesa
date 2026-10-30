using System.Net;
using System.Text.RegularExpressions;
using School.Application.DTOs.Common;
using School.Application.DTOs.Search;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class SearchService : ISearchService
{
    private const int MaxResults = 50;
    private const int SnippetLength = 200;

    private readonly IRepository<Page> _pageRepo;
    private readonly IRepository<Notice> _noticeRepo;
    private readonly IRepository<News> _newsRepo;
    private readonly IRepository<Event> _eventRepo;
    private readonly IRepository<Teacher> _teacherRepo;
    private readonly IRepository<Download> _downloadRepo;
    private readonly IRepository<GalleryAlbum> _albumRepo;

    public SearchService(
        IRepository<Page> pageRepo,
        IRepository<Notice> noticeRepo,
        IRepository<News> newsRepo,
        IRepository<Event> eventRepo,
        IRepository<Teacher> teacherRepo,
        IRepository<Download> downloadRepo,
        IRepository<GalleryAlbum> albumRepo)
    {
        _pageRepo = pageRepo;
        _noticeRepo = noticeRepo;
        _newsRepo = newsRepo;
        _eventRepo = eventRepo;
        _teacherRepo = teacherRepo;
        _downloadRepo = downloadRepo;
        _albumRepo = albumRepo;
    }

    public async Task<ResponseDto<IEnumerable<SearchResultDto>>> SearchAsync(string query)
    {
        try
        {
            query = query?.Trim() ?? string.Empty;
            if (query.Length == 0)
                return ResponseDto<IEnumerable<SearchResultDto>>.Ok(new List<SearchResultDto>());

            // SQL Server's default collation is case-insensitive, so Contains() already ignores case for English.
            var results = new List<SearchResultDto>();

            var pages = await _pageRepo.FindAsync(p => p.IsPublished && p.Status && (
                (p.TitleEn != null && p.TitleEn.Contains(query)) ||
                (p.TitleBn != null && p.TitleBn.Contains(query)) ||
                (p.ContentEn != null && p.ContentEn.Contains(query)) ||
                (p.ContentBn != null && p.ContentBn.Contains(query))));
            results.AddRange(pages.Select(p => new SearchResultDto
            {
                Title = p.TitleEn ?? p.TitleBn ?? "",
                TitleBn = p.TitleBn,
                Description = Snippet(p.ContentEn),
                DescriptionBn = Snippet(p.ContentBn),
                Url = $"/page/{p.Slug}",
                Type = "Page",
                ImageUrl = p.FeaturedImage,
                Date = p.PublishedAt
            }));

            var notices = await _noticeRepo.FindAsync(n => n.IsActive && (
                n.TitleEn.Contains(query) ||
                n.TitleBn.Contains(query) ||
                (n.DescriptionEn != null && n.DescriptionEn.Contains(query)) ||
                (n.DescriptionBn != null && n.DescriptionBn.Contains(query))));
            results.AddRange(notices.Select(n => new SearchResultDto
            {
                Title = n.TitleEn,
                TitleBn = n.TitleBn,
                Description = Snippet(n.DescriptionEn),
                DescriptionBn = Snippet(n.DescriptionBn),
                Url = $"/Notice/Details/{n.Id}",
                Type = "Notice",
                Date = n.PublishDate
            }));

            var newsList = await _newsRepo.FindAsync(n => n.IsActive && (
                n.TitleEn.Contains(query) ||
                n.TitleBn.Contains(query) ||
                (n.ContentEn != null && n.ContentEn.Contains(query)) ||
                (n.ContentBn != null && n.ContentBn.Contains(query))));
            results.AddRange(newsList.Select(n => new SearchResultDto
            {
                Title = n.TitleEn,
                TitleBn = n.TitleBn,
                Description = Snippet(n.ContentEn),
                DescriptionBn = Snippet(n.ContentBn),
                Url = $"/News/Details/{n.Id}",
                Type = "News",
                ImageUrl = n.ImageUrl,
                Date = n.PublishDate
            }));

            var events = await _eventRepo.FindAsync(e => e.IsActive && (
                e.TitleEn.Contains(query) ||
                e.TitleBn.Contains(query) ||
                (e.Location != null && e.Location.Contains(query)) ||
                (e.DescriptionEn != null && e.DescriptionEn.Contains(query)) ||
                (e.DescriptionBn != null && e.DescriptionBn.Contains(query))));
            results.AddRange(events.Select(e => new SearchResultDto
            {
                Title = e.TitleEn,
                TitleBn = e.TitleBn,
                Description = Snippet(e.DescriptionEn),
                DescriptionBn = Snippet(e.DescriptionBn),
                Url = "/Event",
                Type = "Event",
                ImageUrl = e.ImageUrl,
                Date = e.EventDate
            }));

            var teachers = await _teacherRepo.FindAsync(t => t.IsActive && (
                t.NameEn.Contains(query) ||
                t.NameBn.Contains(query) ||
                (t.SubjectEn != null && t.SubjectEn.Contains(query)) ||
                (t.SubjectBn != null && t.SubjectBn.Contains(query))));
            results.AddRange(teachers.Select(t => new SearchResultDto
            {
                Title = t.NameEn,
                TitleBn = t.NameBn,
                Description = string.Join(" • ", new[] { t.SubjectEn, t.QualificationEn }.Where(x => !string.IsNullOrWhiteSpace(x))),
                DescriptionBn = string.Join(" • ", new[] { t.SubjectBn, t.QualificationBn }.Where(x => !string.IsNullOrWhiteSpace(x))),
                Url = $"/Teacher/Details/{t.Id}",
                Type = "Teacher",
                ImageUrl = t.ImageUrl
            }));

            var downloads = await _downloadRepo.FindAsync(d => d.IsActive && (
                d.TitleEn.Contains(query) ||
                d.TitleBn.Contains(query) ||
                (d.DescriptionEn != null && d.DescriptionEn.Contains(query)) ||
                (d.DescriptionBn != null && d.DescriptionBn.Contains(query))));
            results.AddRange(downloads.Select(d => new SearchResultDto
            {
                Title = d.TitleEn,
                TitleBn = d.TitleBn,
                Description = Snippet(d.DescriptionEn),
                DescriptionBn = Snippet(d.DescriptionBn),
                Url = d.FileUrl,
                Type = "Download"
            }));

            var albums = await _albumRepo.FindAsync(a => a.IsActive && (
                a.TitleEn.Contains(query) ||
                a.TitleBn.Contains(query) ||
                (a.DescriptionEn != null && a.DescriptionEn.Contains(query)) ||
                (a.DescriptionBn != null && a.DescriptionBn.Contains(query))));
            results.AddRange(albums.Select(a => new SearchResultDto
            {
                Title = a.TitleEn,
                TitleBn = a.TitleBn,
                Description = Snippet(a.DescriptionEn),
                DescriptionBn = Snippet(a.DescriptionBn),
                Url = $"/Gallery/Album/{a.Id}",
                Type = "Gallery",
                ImageUrl = a.CoverImageUrl
            }));

            return ResponseDto<IEnumerable<SearchResultDto>>.Ok(results.Take(MaxResults).ToList());
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<SearchResultDto>>.Fail(ex);
        }
    }

    /// <summary>Plain-text excerpt of rich-text content.</summary>
    private static string? Snippet(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return null;

        var text = WebUtility.HtmlDecode(Regex.Replace(html, "<[^>]+>", " "));
        text = Regex.Replace(text, @"\s+", " ").Trim();
        return text.Length > SnippetLength ? text[..SnippetLength] + "…" : text;
    }
}
