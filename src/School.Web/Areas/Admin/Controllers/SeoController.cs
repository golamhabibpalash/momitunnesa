using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Settings;
using School.Application.Services.Interfaces;
using School.Web.Helpers;

namespace School.Web.Areas.Admin.Controllers;

public class SeoSettingsModel
{
    [MaxLength(300)]
    [Display(Name = "Meta Description (English)")]
    public string? MetaDescription { get; set; }

    [MaxLength(300)]
    [Display(Name = "Meta Description (Bangla)")]
    public string? MetaDescriptionBn { get; set; }

    [MaxLength(500)]
    [Display(Name = "Meta Keywords")]
    public string? MetaKeywords { get; set; }

    [Display(Name = "Allow search engines to index the website")]
    public bool AllowIndexing { get; set; } = true;

    [MaxLength(200)]
    [Display(Name = "Google Search Console verification code")]
    public string? GoogleSiteVerification { get; set; }

    [MaxLength(30)]
    [RegularExpression(@"^(G-[A-Z0-9]+|UA-\d+-\d+)?$", ErrorMessage = "Use a Google Analytics ID such as G-XXXXXXXXXX.")]
    [Display(Name = "Google Analytics Measurement ID")]
    public string? GoogleAnalyticsId { get; set; }

    public string? OgImageUrl { get; set; }

    [Range(0, 100000)]
    [Display(Name = "Total Students (homepage counter)")]
    public int TotalStudents { get; set; }
}

[Authorize(Roles = "SuperAdmin,Admin")]
public class SeoController : BaseAdminController
{
    private readonly ISettingsService _settingsService;
    private readonly IMediaService _mediaService;

    public SeoController(
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
        var settings = (await _settingsService.GetAllSettingsAsync()).Data?.ToDictionary(s => s.Key, s => s.Value) ?? new();
        string? Get(string key) => settings.GetValueOrDefault(key);

        return View(new SeoSettingsModel
        {
            MetaDescription = Get(SeoSettingKeys.MetaDescription),
            MetaDescriptionBn = Get(SeoSettingKeys.MetaDescriptionBn),
            MetaKeywords = Get(SeoSettingKeys.MetaKeywords),
            AllowIndexing = Get(SeoSettingKeys.AllowIndexing) != "False",
            GoogleSiteVerification = Get(SeoSettingKeys.GoogleSiteVerification),
            GoogleAnalyticsId = Get(SeoSettingKeys.GoogleAnalyticsId),
            OgImageUrl = Get(SeoSettingKeys.OgImageUrl),
            TotalStudents = int.TryParse(Get(SeoSettingKeys.TotalStudents), out var students) ? students : 0
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SeoSettingsModel model, IFormFile? OgImage)
    {
        // People often paste the whole <meta> tag from Search Console; keep only the code.
        if (model.GoogleSiteVerification is { } verification && Regex.Match(verification, "content=\"([^\"]+)\"") is { Success: true } m)
            model.GoogleSiteVerification = m.Groups[1].Value;

        var previousImage = model.OgImageUrl;
        var uploadedUrl = await UploadFileAsync(_mediaService, OgImage, "seo", nameof(model.OgImageUrl));
        if (uploadedUrl is not null)
            model.OgImageUrl = uploadedUrl;

        if (!ModelState.IsValid)
            return View(model);

        var values = new Dictionary<string, string?>
        {
            [SeoSettingKeys.MetaDescription] = model.MetaDescription?.Trim(),
            [SeoSettingKeys.MetaDescriptionBn] = model.MetaDescriptionBn?.Trim(),
            [SeoSettingKeys.MetaKeywords] = model.MetaKeywords?.Trim(),
            [SeoSettingKeys.AllowIndexing] = model.AllowIndexing ? "True" : "False",
            [SeoSettingKeys.GoogleSiteVerification] = model.GoogleSiteVerification?.Trim(),
            [SeoSettingKeys.GoogleAnalyticsId] = model.GoogleAnalyticsId?.Trim(),
            [SeoSettingKeys.OgImageUrl] = model.OgImageUrl,
            [SeoSettingKeys.TotalStudents] = model.TotalStudents.ToString()
        };

        foreach (var (key, value) in values)
        {
            var result = await _settingsService.UpdateSettingAsync(new SiteSettingUpdateDto { Key = key, Value = value ?? "" });
            if (!result.Success)
            {
                SetErrorNotification(result.Message ?? "Failed to save SEO settings.");
                return View(model);
            }
        }

        if (uploadedUrl is not null)
            await DeleteUploadAsync(_mediaService, previousImage);

        SetSuccessNotification("SEO settings saved.");
        return RedirectToAction(nameof(Index));
    }
}
