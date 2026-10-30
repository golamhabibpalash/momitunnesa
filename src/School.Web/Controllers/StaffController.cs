using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Web.Models;

namespace School.Web.Controllers;

public class StaffController : BaseController
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _staffService.GetActiveStaffAsync();

        if (!result.Success || result.Data is null)
            return View(new StaffListViewModel());

        var staff = result.Data.OrderBy(s => s.SortOrder).Select(s => new StaffItem
        {
            Id = s.Id,
            Name = GetLocalizedContent(s.NameEn, s.NameBn),
            NameBn = s.NameBn,
            Designation = GetLocalizedContent(s.PositionEn ?? "", string.IsNullOrWhiteSpace(s.PositionBn) ? s.PositionEn ?? "" : s.PositionBn),
            Department = s.Department,
            ImageUrl = s.ImageUrl,
            Email = s.Email,
            Phone = s.Phone
        }).ToList();

        var viewModel = new StaffListViewModel
        {
            Staff = staff
        };

        return View(viewModel);
    }
}
