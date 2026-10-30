using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Domain.Enums;
using School.Web.Models;

namespace School.Web.Controllers;

public class NoticeController : BaseController
{
    private readonly INoticeService _noticeService;
    private const int PageSize = 10;

    public NoticeController(INoticeService noticeService)
    {
        _noticeService = noticeService;
    }

    public async Task<IActionResult> Index(NoticeCategory? category, int page = 1)
    {
        var result = category.HasValue
            ? await _noticeService.GetNoticesByCategoryAsync(category.Value)
            : await _noticeService.GetActiveNoticesAsync();

        if (!result.Success || result.Data is null)
            return View(new NoticeListViewModel());

        var notices = result.Data.Select(n => new NoticeItem
        {
            Id = n.Id,
            Title = GetLocalizedContent(n.TitleEn, n.TitleBn),
            TitleBn = n.TitleBn,
            Description = GetLocalizedContent(n.DescriptionEn ?? "", n.DescriptionBn ?? ""),
            Category = n.NoticeCategory,
            PublishDate = n.PublishDate,
            IsFeatured = n.IsFeatured,
            Attachments = n.AttachmentUrl is not null
                ? new List<AttachmentItem> { new() { Url = n.AttachmentUrl, Name = n.TitleEn } }
                : null
        }).ToList();

        var totalItems = notices.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
        var pagedNotices = notices.Skip((page - 1) * PageSize).Take(PageSize).ToList();

        var viewModel = new NoticeListViewModel
        {
            Notices = pagedNotices,
            CurrentPage = page,
            TotalPages = totalPages,
            Category = category?.ToString()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _noticeService.GetByIdAsync(id);

        if (!result.Success || result.Data is null)
            return NotFoundResponse("Notice not found.");

        var notice = result.Data;
        await _noticeService.IncrementViewCountAsync(id);

        var viewModel = new NoticeViewModel
        {
            Id = notice.Id,
            Title = GetLocalizedContent(notice.TitleEn, notice.TitleBn),
            TitleBn = notice.TitleBn,
            Content = GetLocalizedContent(notice.DescriptionEn ?? "", notice.DescriptionBn ?? ""),
            ContentBn = notice.DescriptionBn,
            Category = notice.NoticeCategory,
            PublishDate = notice.PublishDate,
            IsFeatured = notice.IsFeatured,
            Attachments = notice.AttachmentUrl is not null ? new List<AttachmentItem>
            {
                new() { Url = notice.AttachmentUrl, Name = notice.TitleEn }
            } : null
        };

        return View(viewModel);
    }
}
