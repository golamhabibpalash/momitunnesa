using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Settings;
using School.Application.Services.Interfaces;
using School.Web.Helpers;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class SettingsController : BaseAdminController
{
    private readonly ISettingsService _settingsService;
    private readonly IMediaService _mediaService;

    public SettingsController(
        ISettingsService settingsService,
        IMediaService mediaService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _settingsService = settingsService;
        _mediaService = mediaService;
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

        // Both names are non-nullable on the entity, so an empty post would overwrite
        // the stored value rather than being skipped by the mapper.
        if (string.IsNullOrWhiteSpace(model.SchoolNameEn) || string.IsNullOrWhiteSpace(model.SchoolNameBn))
        {
            SetErrorNotification("School name (English and Bangla) is required.");
            return RedirectToAction(nameof(Index));
        }

        // The fields this form actually renders are coalesced to empty so that clearing
        // a box clears the stored value (model binding turns empty input into null,
        // which the mapper skips). The rest stay null and keep whatever is stored.
        var updateDto = new SchoolInfoUpdateDto
        {
            SchoolNameEn = model.SchoolNameEn,
            SchoolNameBn = model.SchoolNameBn,
            SloganEn = model.SloganEn,
            SloganBn = model.SloganBn,
            AddressEn = model.AddressEn,
            AddressBn = model.AddressBn,
            Phone = model.Phone ?? string.Empty,
            Email = model.Email ?? string.Empty,
            Website = model.Website ?? string.Empty,
            LogoUrl = model.LogoUrl,
            FaviconUrl = model.FaviconUrl,
            GoogleMapEmbedUrl = model.GoogleMapEmbedUrl,
            FooterTextEn = model.FooterTextEn ?? string.Empty,
            FooterTextBn = model.FooterTextBn ?? string.Empty,
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
    public async Task<IActionResult> UpdateAbout(SchoolInfoDto model)
    {
        var currentResult = await _settingsService.GetSchoolInfoAsync();
        if (!currentResult.Success || currentResult.Data is null)
        {
            SetErrorNotification("School information has not been set up yet.");
            return RedirectToAction(nameof(Index));
        }

        var current = currentResult.Data;

        // Only the About fields are set. Everything else stays null so the mapper
        // leaves it untouched; the names are required on the entity, so they are
        // carried over from the current record.
        //
        // This form owns every About field, so an empty box means "clear it". Model
        // binding turns an empty form value into null, which the mapper would skip,
        // so the nulls are coalesced to empty strings to make clearing work.
        var updateDto = new SchoolInfoUpdateDto
        {
            SchoolNameEn = current.SchoolNameEn,
            SchoolNameBn = current.SchoolNameBn,
            AboutShortEn = model.AboutShortEn ?? string.Empty,
            AboutShortBn = model.AboutShortBn ?? string.Empty,
            HistoryEn = model.HistoryEn ?? string.Empty,
            HistoryBn = model.HistoryBn ?? string.Empty,
            MissionEn = model.MissionEn ?? string.Empty,
            MissionBn = model.MissionBn ?? string.Empty,
            VisionEn = model.VisionEn ?? string.Empty,
            VisionBn = model.VisionBn ?? string.Empty,
            ObjectiveEn = model.ObjectiveEn ?? string.Empty,
            ObjectiveBn = model.ObjectiveBn ?? string.Empty,
            FuturePlanEn = model.FuturePlanEn ?? string.Empty,
            FuturePlanBn = model.FuturePlanBn ?? string.Empty
        };

        var result = await _settingsService.UpdateSchoolInfoAsync(updateDto);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update the About content.");
        }
        else
        {
            SetSuccessNotification("About content updated successfully.");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateBranding(
        IFormFile? LogoFile,
        IFormFile? FaviconFile,
        bool RemoveLogo = false,
        bool RemoveFavicon = false)
    {
        var currentResult = await _settingsService.GetSchoolInfoAsync();
        if (!currentResult.Success || currentResult.Data is null)
        {
            SetErrorNotification("School information has not been set up yet.");
            return RedirectToAction(nameof(Index));
        }

        var current = currentResult.Data;

        var logoUrl = await ResolveBrandingImageAsync(LogoFile, RemoveLogo, current.LogoUrl, "logo");
        if (logoUrl.Error is not null)
        {
            SetErrorNotification(logoUrl.Error);
            return RedirectToAction(nameof(Index));
        }

        var faviconUrl = await ResolveBrandingImageAsync(FaviconFile, RemoveFavicon, current.FaviconUrl, "favicon");
        if (faviconUrl.Error is not null)
        {
            SetErrorNotification(faviconUrl.Error);
            return RedirectToAction(nameof(Index));
        }

        // Only the branding fields are set; every other member stays null so the
        // mapper leaves the stored value untouched. The names are required on the
        // entity, so they are carried over from the current record.
        var updateDto = new SchoolInfoUpdateDto
        {
            SchoolNameEn = current.SchoolNameEn,
            SchoolNameBn = current.SchoolNameBn,
            LogoUrl = logoUrl.Value,
            FaviconUrl = faviconUrl.Value
        };

        var result = await _settingsService.UpdateSchoolInfoAsync(updateDto);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update branding.");
            return RedirectToAction(nameof(Index));
        }

        // The old files are only removed once the new paths are safely persisted.
        if (logoUrl.FileToDelete is not null)
            await _mediaService.DeleteAsync(logoUrl.FileToDelete);
        if (faviconUrl.FileToDelete is not null)
            await _mediaService.DeleteAsync(faviconUrl.FileToDelete);

        SetSuccessNotification("Branding updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Works out the new value for a branding image field: the uploaded file's URL,
    /// an empty string when it is being removed, or null to leave the stored value alone.
    /// </summary>
    private async Task<BrandingImageResult> ResolveBrandingImageAsync(
        IFormFile? file, bool remove, string? currentUrl, string label)
    {
        if (file is not null && file.Length > 0)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(extension))
                return new BrandingImageResult { Error = $"The {label} must be an image ({string.Join(", ", AllowedImageExtensions)})." };

            var uploadResult = await _mediaService.UploadAsync(file, "branding");
            if (!uploadResult.Success || string.IsNullOrEmpty(uploadResult.Url))
                return new BrandingImageResult { Error = uploadResult.Error ?? $"Failed to upload the {label}." };

            return new BrandingImageResult { Value = uploadResult.Url, FileToDelete = Removable(currentUrl) };
        }

        if (remove)
            return new BrandingImageResult { Value = string.Empty, FileToDelete = Removable(currentUrl) };

        return new BrandingImageResult();
    }

    /// <summary>
    /// Only files this uploader created may be deleted; anything else (a hand-placed
    /// static asset such as /favicon.ico, or an external URL) is left in place.
    /// </summary>
    private static string? Removable(string? url) =>
        url is not null && url.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase) ? url : null;

    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".ico" };

    private sealed class BrandingImageResult
    {
        public string? Value { get; init; }
        public string? FileToDelete { get; init; }
        public string? Error { get; init; }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSocialLink(SocialLinkCreateDto model)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Platform) || string.IsNullOrWhiteSpace(model.Url))
        {
            SetErrorNotification("Platform and URL are both required.");
            return RedirectToAction(nameof(Index));
        }

        // The icon is derived from the platform so the admin never has to know
        // Font Awesome class names.
        model.Icon = SocialPlatformHelper.GetIcon(model.Platform);

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
    public async Task<IActionResult> UpdateSocialLink(SocialLinkDto model)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Platform) || string.IsNullOrWhiteSpace(model.Url))
        {
            SetErrorNotification("Platform and URL are both required.");
            return RedirectToAction(nameof(Index));
        }

        model.Icon = SocialPlatformHelper.GetIcon(model.Platform);

        var result = await _settingsService.UpdateSocialLinkAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update social link.");
        }
        else
        {
            SetSuccessNotification("Social link updated successfully.");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSocialLink(int id)
    {
        var result = await _settingsService.DeleteSocialLinkAsync(id);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to delete social link.");
        }
        else
        {
            SetSuccessNotification("Social link deleted successfully.");
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
