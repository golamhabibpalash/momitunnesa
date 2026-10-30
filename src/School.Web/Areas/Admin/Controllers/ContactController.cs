using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class ContactController : BaseAdminController
{
    private readonly IContactService _contactService;
    private const int PageSize = 20;

    public ContactController(
        IContactService contactService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _contactService = contactService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _contactService.GetAllMessagesAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load messages.");
            return View(new List<School.Application.DTOs.Contact.ContactMessageDto>());
        }

        var messages = result.Data.OrderByDescending(m => m.CreatedAt).ToList();
        var totalPages = (int)Math.Ceiling(messages.Count / (double)PageSize);
        var pagedItems = messages.Skip((page - 1) * PageSize).Take(PageSize).ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(pagedItems);
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _contactService.GetMessageByIdAsync(id);

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification("Message not found.");
            return RedirectToAction(nameof(Index));
        }

        if (!result.Data.IsRead)
        {
            await _contactService.MarkAsReadAsync(id);
            result.Data.IsRead = true;
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _contactService.DeleteAsync(id);

        if (!result.Success)
            SetErrorNotification(result.Message ?? "Failed to delete message.");
        else
            SetSuccessNotification("Message deleted successfully.");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var result = await _contactService.MarkAsReadAsync(id);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to mark message as read.");
        }
        else
        {
            SetSuccessNotification("Message marked as read.");
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}
