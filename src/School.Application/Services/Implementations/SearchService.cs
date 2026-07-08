using School.Application.DTOs.Common;
using School.Application.DTOs.Search;
using School.Application.Services.Interfaces;
using School.Domain.Entities;
using School.Domain.Interfaces;

namespace School.Application.Services.Implementations;

public class SearchService : ISearchService
{
    private readonly IRepository<Page> _pageRepo;
    private readonly IRepository<Notice> _noticeRepo;
    private readonly IRepository<News> _newsRepo;
    private readonly IRepository<Teacher> _teacherRepo;
    private readonly IRepository<Download> _downloadRepo;
    private readonly IRepository<GalleryAlbum> _albumRepo;

    public SearchService(
        IRepository<Page> pageRepo,
        IRepository<Notice> noticeRepo,
        IRepository<News> newsRepo,
        IRepository<Teacher> teacherRepo,
        IRepository<Download> downloadRepo,
        IRepository<GalleryAlbum> albumRepo)
    {
        _pageRepo = pageRepo;
        _noticeRepo = noticeRepo;
        _newsRepo = newsRepo;
        _teacherRepo = teacherRepo;
        _downloadRepo = downloadRepo;
        _albumRepo = albumRepo;
    }

    public async Task<ResponseDto<IEnumerable<SearchResultDto>>> SearchAsync(string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return ResponseDto<IEnumerable<SearchResultDto>>.Ok(new List<SearchResultDto>());

            var lowerQuery = query.ToLowerInvariant();
            var results = new List<SearchResultDto>();

            var pages = await _pageRepo.FindAsync(p =>
                (p.TitleEn != null && p.TitleEn.ToLower().Contains(lowerQuery)) ||
                (p.TitleBn != null && p.TitleBn.Contains(query)) ||
                (p.ContentEn != null && p.ContentEn.ToLower().Contains(lowerQuery)) ||
                (p.ContentBn != null && p.ContentBn.Contains(query)));
            results.AddRange(pages.Where(p => p.Status).Select(p => new SearchResultDto
            {
                Title = p.TitleEn ?? p.TitleBn ?? "",
                Description = p.ContentEn?.Length > 200 ? p.ContentEn[..200] + "..." : p.ContentEn,
                Url = $"/page/{p.Slug}",
                Type = "Page",
                ImageUrl = p.FeaturedImage
            }));

            var notices = await _noticeRepo.FindAsync(n =>
                n.TitleEn.ToLower().Contains(lowerQuery) ||
                n.TitleBn.Contains(query) ||
                (n.DescriptionEn != null && n.DescriptionEn.ToLower().Contains(lowerQuery)) ||
                (n.DescriptionBn != null && n.DescriptionBn.Contains(query)));
            results.AddRange(notices.Where(n => n.IsActive).Select(n => new SearchResultDto
            {
                Title = n.TitleEn,
                Description = n.DescriptionEn?.Length > 200 ? n.DescriptionEn[..200] + "..." : n.DescriptionEn,
                Url = $"/notice/{n.Id}",
                Type = "Notice",
                ImageUrl = null
            }));

            var newsList = await _newsRepo.FindAsync(n =>
                n.TitleEn.ToLower().Contains(lowerQuery) ||
                n.TitleBn.Contains(query) ||
                (n.ContentEn != null && n.ContentEn.ToLower().Contains(lowerQuery)) ||
                (n.ContentBn != null && n.ContentBn.Contains(query)));
            results.AddRange(newsList.Where(n => n.IsActive).Select(n => new SearchResultDto
            {
                Title = n.TitleEn,
                Description = n.ContentEn?.Length > 200 ? n.ContentEn[..200] + "..." : n.ContentEn,
                Url = $"/news/{n.Id}",
                Type = "News",
                ImageUrl = n.ImageUrl
            }));

            var teachers = await _teacherRepo.FindAsync(t =>
                t.NameEn.ToLower().Contains(lowerQuery) ||
                t.NameBn.Contains(query) ||
                (t.SubjectEn != null && t.SubjectEn.ToLower().Contains(lowerQuery)) ||
                (t.SubjectBn != null && t.SubjectBn.Contains(query)));
            results.AddRange(teachers.Where(t => t.IsActive).Select(t => new SearchResultDto
            {
                Title = t.NameEn,
                Description = t.QualificationEn,
                Url = $"/teacher/{t.Id}",
                Type = "Teacher",
                ImageUrl = t.ImageUrl
            }));

            var downloads = await _downloadRepo.FindAsync(d =>
                d.TitleEn.ToLower().Contains(lowerQuery) ||
                d.TitleBn.Contains(query) ||
                (d.DescriptionEn != null && d.DescriptionEn.ToLower().Contains(lowerQuery)) ||
                (d.DescriptionBn != null && d.DescriptionBn.Contains(query)));
            results.AddRange(downloads.Where(d => d.IsActive).Select(d => new SearchResultDto
            {
                Title = d.TitleEn,
                Description = d.DescriptionEn,
                Url = $"/download/{d.Id}",
                Type = "Download",
                ImageUrl = null
            }));

            var albums = await _albumRepo.FindAsync(a =>
                a.TitleEn.ToLower().Contains(lowerQuery) ||
                a.TitleBn.Contains(query) ||
                (a.DescriptionEn != null && a.DescriptionEn.ToLower().Contains(lowerQuery)) ||
                (a.DescriptionBn != null && a.DescriptionBn.Contains(query)));
            results.AddRange(albums.Where(a => a.IsActive).Select(a => new SearchResultDto
            {
                Title = a.TitleEn,
                Description = a.DescriptionEn,
                Url = $"/gallery/{a.Id}",
                Type = "Gallery",
                ImageUrl = a.CoverImageUrl
            }));

            return ResponseDto<IEnumerable<SearchResultDto>>.Ok(results.Take(50));
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<SearchResultDto>>.Fail(ex);
        }
    }
}
