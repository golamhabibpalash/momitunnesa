using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Notice;
using School.Application.Services.Interfaces;
using School.Domain.Enums;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class NoticeController : BaseAdminController
{
    private readonly INoticeService _noticeService;
    private readonly IMediaService _mediaService;

    public NoticeController(
        INoticeService noticeService,
        IMediaService mediaService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _noticeService = noticeService;
        _mediaService = mediaService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _noticeService.GetAllAsync();
        const int pageSize = 20;

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load notices.");
            return View(new List<NoticeDto>());
        }

        var notices = result.Data.OrderByDescending(n => n.PublishDate).ToList();
        var totalPages = (int)Math.Ceiling(notices.Count / (double)pageSize);
        var pagedItems = notices.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(pagedItems);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new NoticeCreateDto { PublishDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NoticeCreateDto model, IFormFile? Attachment)
    {
        var uploadedUrl = await UploadFileAsync(_mediaService, Attachment, "notices", nameof(model.AttachmentUrl));
        if (uploadedUrl is not null)
            model.AttachmentUrl = uploadedUrl;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _noticeService.CreateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create notice.");
            return View(model);
        }

        SetSuccessNotification("Notice created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _noticeService.GetByIdAsync(id);

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification("Notice not found.");
            return RedirectToAction(nameof(Index));
        }

        var notice = result.Data;
        var dto = new NoticeCreateDto
        {
            TitleEn = notice.TitleEn,
            TitleBn = notice.TitleBn,
            DescriptionEn = notice.DescriptionEn,
            DescriptionBn = notice.DescriptionBn,
            NoticeCategory = Enum.TryParse<NoticeCategory>(notice.NoticeCategory, out var cat) ? cat : NoticeCategory.General,
            AttachmentUrl = notice.AttachmentUrl,
            PublishDate = notice.PublishDate,
            ExpiryDate = notice.ExpiryDate,
            IsActive = notice.IsActive,
            IsFeatured = notice.IsFeatured
        };

        ViewBag.NoticeId = id;
        return View("Create", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NoticeCreateDto model, IFormFile? Attachment, bool removeAttachment = false)
    {
        var previousUrl = model.AttachmentUrl;
        if (removeAttachment)
            model.AttachmentUrl = null;

        var uploadedUrl = await UploadFileAsync(_mediaService, Attachment, "notices", nameof(model.AttachmentUrl));
        if (uploadedUrl is not null)
            model.AttachmentUrl = uploadedUrl;

        if (!ModelState.IsValid)
        {
            ViewBag.NoticeId = id;
            return View("Create", model);
        }

        var updateDto = new NoticeUpdateDto
        {
            Id = id,
            TitleEn = model.TitleEn,
            TitleBn = model.TitleBn,
            DescriptionEn = model.DescriptionEn,
            DescriptionBn = model.DescriptionBn,
            NoticeCategory = model.NoticeCategory,
            AttachmentUrl = model.AttachmentUrl,
            PublishDate = model.PublishDate,
            ExpiryDate = model.ExpiryDate,
            IsActive = model.IsActive,
            IsFeatured = model.IsFeatured
        };

        var result = await _noticeService.UpdateAsync(updateDto);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update notice.");
            ViewBag.NoticeId = id;
            return View("Create", model);
        }

        if (previousUrl != model.AttachmentUrl)
            await DeleteUploadAsync(_mediaService, previousUrl);

        SetSuccessNotification("Notice updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _noticeService.DeleteAsync(id);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to delete notice.");
        }
        else
        {
            SetSuccessNotification("Notice deleted successfully.");
        }

        return RedirectToAction(nameof(Index));
    }
}
