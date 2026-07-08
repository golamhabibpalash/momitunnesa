using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;

namespace School.Web.ViewComponents;

public class LatestNoticeViewComponent : ViewComponent
{
    private readonly INoticeService _noticeService;

    public LatestNoticeViewComponent(INoticeService noticeService)
    {
        _noticeService = noticeService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int count = 5)
    {
        var result = await _noticeService.GetActiveNoticesAsync();
        var notices = (result.Data ?? Enumerable.Empty<School.Application.DTOs.Notice.NoticeDto>())
            .Take(count);
        return View(notices);
    }
}
