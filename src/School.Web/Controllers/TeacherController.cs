using Microsoft.AspNetCore.Mvc;
using School.Application.Services.Interfaces;
using School.Domain.Enums;
using School.Web.Models;

namespace School.Web.Controllers;

public class TeacherController : BaseController
{
    private readonly ITeacherService _teacherService;

    public TeacherController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    public async Task<IActionResult> Index(Designation? designation)
    {
        var result = await _teacherService.GetActiveTeachersAsync();

        if (!result.Success || result.Data is null)
            return View(new TeacherListViewModel());

        var teachers = result.Data
            .Where(t => !designation.HasValue || t.Designation == designation.Value.ToString())
            .Select(t => new TeacherItem
            {
                Id = t.Id,
                Name = GetLocalizedContent(t.NameEn, t.NameBn),
                NameBn = t.NameBn,
                Designation = t.Designation,
                DesignationBn = t.Designation,
                Qualification = GetLocalizedContent(t.QualificationEn ?? "", t.QualificationBn ?? ""),
                ImageUrl = t.ImageUrl,
                Subject = GetLocalizedContent(t.SubjectEn ?? "", t.SubjectBn ?? "")
            }).ToList();

        var viewModel = new TeacherListViewModel
        {
            Teachers = teachers,
            Designation = designation?.ToString()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _teacherService.GetByIdAsync(id);

        if (!result.Success || result.Data is null)
            return NotFoundResponse("Teacher not found.");

        var teacher = result.Data;
        var viewModel = new TeacherViewModel
        {
            Id = teacher.Id,
            Name = GetLocalizedContent(teacher.NameEn, teacher.NameBn),
            NameBn = teacher.NameBn,
            Designation = teacher.Designation,
            Qualification = GetLocalizedContent(teacher.QualificationEn ?? "", teacher.QualificationBn ?? ""),
            ImageUrl = teacher.ImageUrl,
            Email = teacher.Email,
            Phone = teacher.Phone,
            Subject = GetLocalizedContent(teacher.SubjectEn ?? "", teacher.SubjectBn ?? ""),
            SubjectBn = teacher.SubjectBn,
            JoiningDate = teacher.JoiningDate
        };

        return View(viewModel);
    }
}
