using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Web.Models;

namespace School.Web.Controllers;

public class EventController : BaseController
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _eventService.GetUpcomingEventsAsync();

        if (!result.Success || result.Data is null)
            return View(new EventListViewModel());

        var events = result.Data.Select(e => new EventItem
        {
            Id = e.Id,
            Title = GetLocalizedContent(e.TitleEn, e.TitleBn),
            TitleBn = e.TitleBn,
            Description = GetLocalizedContent(e.DescriptionEn ?? "", e.DescriptionBn ?? ""),
            EventDate = e.EventDate,
            Location = e.Location,
            StartTime = e.StartTime?.ToString(@"hh\:mm"),
            EndTime = e.EndTime?.ToString(@"hh\:mm")
        }).ToList();

        var viewModel = new EventListViewModel
        {
            Events = events,
            TotalPages = 1,
            CurrentPage = 1
        };

        return View(viewModel);
    }
}
