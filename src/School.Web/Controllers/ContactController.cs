using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Contact;
using School.Application.Services.Interfaces;

namespace School.Web.Controllers;

public class ContactController : BaseController
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    public IActionResult Index()
    {
        return View(new ContactCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(ContactCreateDto model)
    {
        if (!ModelState.IsValid)
            return View("Index", model);

        var result = await _contactService.CreateAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Failed to send message.");
            return View("Index", model);
        }

        TempData["SuccessMessage"] = "Your message has been sent successfully. We will get back to you soon.";
        return RedirectToAction(nameof(Index));
    }
}
