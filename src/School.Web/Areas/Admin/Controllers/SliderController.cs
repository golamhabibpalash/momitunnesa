using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Slider;
using School.Application.Services.Interfaces;

namespace School.Web.Areas.Admin.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class SliderController : BaseAdminController
{
    private readonly ISliderService _sliderService;
    private readonly IMediaService _mediaService;

    public SliderController(
        ISliderService sliderService,
        IMediaService mediaService,
        ILogger<BaseAdminController> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger, httpContextAccessor)
    {
        _sliderService = sliderService;
        _mediaService = mediaService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _sliderService.GetAllAsync();

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification(result.Message ?? "Failed to load sliders.");
            return View(new List<SliderDto>());
        }

        return View(result.Data.OrderBy(s => s.SortOrder).ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new SliderCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SliderCreateDto model, IFormFile? Image)
    {
        if (Image is not null && Image.Length > 0)
        {
            var uploadResult = await _mediaService.UploadAsync(Image, "sliders");
            if (uploadResult.Success)
            {
                model.ImageUrl = uploadResult.Url ?? "";
                ModelState.Remove("ImageUrl");
            }
            else
            {
                SetErrorNotification(uploadResult.Error ?? "Failed to upload image.");
                return View(model);
            }
        }

        if (!ModelState.IsValid)
            return View(model);

        var result = await _sliderService.CreateAsync(model);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to create slider.");
            return View(model);
        }

        SetSuccessNotification("Slider created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _sliderService.GetByIdAsync(id);

        if (!result.Success || result.Data is null)
        {
            SetErrorNotification("Slider not found.");
            return RedirectToAction(nameof(Index));
        }

        var slider = result.Data;
        var dto = new SliderCreateDto
        {
            TitleEn = slider.TitleEn,
            TitleBn = slider.TitleBn,
            SubtitleEn = slider.SubtitleEn,
            SubtitleBn = slider.SubtitleBn,
            ImageUrl = slider.ImageUrl,
            LinkUrl = slider.LinkUrl,
            SortOrder = slider.SortOrder,
            IsActive = slider.IsActive
        };

        ViewBag.SliderId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SliderCreateDto model, IFormFile? Image, string? removeImage)
    {
        if (removeImage == "true")
        {
            if (!string.IsNullOrEmpty(model.ImageUrl))
                await _mediaService.DeleteAsync(model.ImageUrl);
            model.ImageUrl = "";
            ModelState.Remove("ImageUrl");
        }

        if (Image is not null && Image.Length > 0)
        {
            var uploadResult = await _mediaService.UploadAsync(Image, "sliders");
            if (uploadResult.Success)
            {
                if (!string.IsNullOrEmpty(model.ImageUrl))
                    await _mediaService.DeleteAsync(model.ImageUrl);
                model.ImageUrl = uploadResult.Url ?? "";
                ModelState.Remove("ImageUrl");
            }
            else
            {
                SetErrorNotification(uploadResult.Error ?? "Failed to upload image.");
                ViewBag.SliderId = id;
                return View(model);
            }
        }

        if (!ModelState.IsValid)
        {
            ViewBag.SliderId = id;
            return View(model);
        }

        var updateDto = new SliderUpdateDto
        {
            Id = id,
            TitleEn = model.TitleEn,
            TitleBn = model.TitleBn,
            SubtitleEn = model.SubtitleEn,
            SubtitleBn = model.SubtitleBn,
            ImageUrl = model.ImageUrl,
            LinkUrl = model.LinkUrl,
            SortOrder = model.SortOrder,
            IsActive = model.IsActive
        };

        var result = await _sliderService.UpdateAsync(updateDto);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to update slider.");
            ViewBag.SliderId = id;
            return View(model);
        }

        SetSuccessNotification("Slider updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _sliderService.DeleteAsync(id);

        if (!result.Success)
        {
            SetErrorNotification(result.Message ?? "Failed to delete slider.");
        }
        else
        {
            SetSuccessNotification("Slider deleted successfully.");
        }

        return RedirectToAction(nameof(Index));
    }
}
