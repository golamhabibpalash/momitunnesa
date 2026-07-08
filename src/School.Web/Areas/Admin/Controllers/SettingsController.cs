using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Settings;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class SettingsController : BaseAdminController
{
    private readonly ISettingsService _settingsService;

    public SettingsController(
        ISettingsService settingsService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _settingsService = settingsService;
    }

    public async Task<IActionResult> Index()
    {
        var schoolInfoResult = await _settingsService.GetSchoolInfoAsync();
        var socialLinksResult = await _settingsService.GetAllSocialLinksAsync();
        var settingsResult = await _settingsService.GetAllSettingsAsync();

        if (schoolInfoResult.Success && schoolInfoResult.Data is not null)
            ViewBag.SchoolInfo = schoolInfoResult.Data;

        if (socialLinksResult.Success && socialLinksResult.Data is not null)
            ViewBag.SocialLinks = socialLinksResult.Data;

        if (settingsResult.Success && settingsResult.Data is not null)
            ViewBag.SiteSettings = settingsResult.Data;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateGeneral(SchoolInfoDto model)
    {
        if (!ModelState.IsValid)
        {
            SetErrorNotification("Please fix the validation errors.");
            return RedirectToAction(nameof(Index));
        }

        var updateDto = new SchoolInfoUpdateDto
        {
            SchoolNameEn = model.SchoolNameEn,
            SchoolNameBn = model.SchoolNameBn,
            SloganEn = model.SloganEn,
            SloganBn = model.SloganBn,
            AddressEn = model.AddressEn,
            AddressBn = model.AddressBn,
            Phone = model.Phone,
            Email = model.Email,
            Website = model.Website,
            LogoUrl = model.LogoUrl,
            FaviconUrl = model.FaviconUrl,
            GoogleMapEmbedUrl = model.GoogleMapEmbedUrl,
            FooterTextEn = model.FooterTextEn,
            FooterTextBn = model.FooterTextBn,
            AboutShortEn = model.AboutShortEn,
            AboutShortBn = model.AboutShortBn,
            MissionEn = model.MissionEn,
            MissionBn = model.MissionBn,
            VisionEn = model.VisionEn,
            VisionBn = model.VisionBn,
            HistoryEn = model.HistoryEn,
            HistoryBn = model.HistoryBn,
            ObjectiveEn = model.ObjectiveEn,
            ObjectiveBn = model.ObjectiveBn,
            FuturePlanEn = model.FuturePlanEn,
            FuturePlanBn = model.FuturePlanBn
        };

        var result = await _settingsService.UpdateSchoolInfoAsync(updateDto);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update school information.");
        }
        else
        {
            SetSuccessNotification("School information updated successfully.");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSocialLink(SocialLinkCreateDto model)
    {
        if (!ModelState.IsValid)
        {
            SetErrorNotification("Please fix the validation errors.");
            return RedirectToAction(nameof(Index));
        }

        var result = await _settingsService.CreateSocialLinkAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to add social link.");
        }
        else
        {
            SetSuccessNotification("Social link added successfully.");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSeo(Dictionary<string, string> model)
    {
        var result = true;
        foreach (var kvp in model)
        {
            var settingDto = new SiteSettingUpdateDto
            {
                Key = kvp.Key,
                Value = kvp.Value
            };
            var settingResult = await _settingsService.UpdateSettingAsync(settingDto);
            if (!settingResult.Success)
                result = false;
        }

        if (!result)
        {
            SetErrorNotification("Failed to update some SEO settings.");
        }
        else
        {
            SetSuccessNotification("SEO settings updated successfully.");
        }

        return RedirectToAction(nameof(Index));
    }
}
