using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Academic;
using School.Application.Services.Interfaces;

namespace School.Web.Controllers;

public class AcademicController : BaseController
{
    private readonly IPageService _pageService;
    private readonly IAcademicService _academicService;

    public AcademicController(IPageService pageService, IAcademicService academicService)
    {
        _pageService = pageService;
        _academicService = academicService;
    }

    public async Task<IActionResult> Calendar(int? year)
    {
        await LoadPageContentAsync("academic-calendar");
        var items = (await _academicService.GetCalendarEventsAsync(activeOnly: true)).Data?.ToList() ?? new List<AcademicCalendarDto>();

        var years = items.Select(i => i.EventDate.Year).Distinct().OrderBy(y => y).ToList();
        var selectedYear = year ?? (years.Contains(DateTime.Today.Year) ? DateTime.Today.Year : years.LastOrDefault(DateTime.Today.Year));
        ViewBag.Years = years;
        ViewBag.SelectedYear = selectedYear;

        return View(items.Where(i => i.EventDate.Year == selectedYear).ToList());
    }

    public async Task<IActionResult> Routine(string? className, string? section)
    {
        await LoadPageContentAsync("class-routine");
        var items = (await _academicService.GetRoutinesAsync(activeOnly: true)).Data?.ToList() ?? new List<RoutineDto>();

        var classes = items
            .Select(r => (r.ClassNameEn, r.Section))
            .Distinct()
            .OrderBy(c => c.ClassNameEn).ThenBy(c => c.Section)
            .ToList();

        var selected = classes.FirstOrDefault(c => c.ClassNameEn == className && (string.IsNullOrEmpty(section) || c.Section == section));
        if (selected == default && classes.Count > 0)
            selected = classes[0];

        ViewBag.Classes = classes;
        ViewBag.SelectedClass = selected.ClassNameEn;
        ViewBag.SelectedSection = selected.Section;

        return View(items.Where(r => r.ClassNameEn == selected.ClassNameEn && r.Section == selected.Section).ToList());
    }

    public async Task<IActionResult> Syllabus()
    {
        await LoadPageContentAsync("syllabus");
        var items = (await _academicService.GetSyllabusesAsync(activeOnly: true)).Data?.ToList() ?? new List<SyllabusDto>();
        return View(items);
    }

    public async Task<IActionResult> Subjects()
    {
        await LoadPageContentAsync("subjects");
        var items = (await _academicService.GetSubjectsAsync(activeOnly: true)).Data?.ToList() ?? new List<SubjectDto>();
        return View(items);
    }

    /// <summary>An optional CMS page with the given slug is shown above the data.</summary>
    private async Task LoadPageContentAsync(string slug)
    {
        var pageResult = await _pageService.GetBySlugAsync(slug);
        if (pageResult.Success && pageResult.Data is { IsPublished: true, Status: true } page)
            ViewBag.Content = GetLocalizedContent(page.ContentEn ?? "", page.ContentBn ?? "");
    }
}
