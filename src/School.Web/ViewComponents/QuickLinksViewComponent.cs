using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School.Domain.Entities;
using School.Infrastructure.Data;

namespace School.Web.ViewComponents;

public class QuickLinksViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public QuickLinksViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var links = await _context.ImportantLinks
            .Where(l => l.IsActive && !l.IsDeleted)
            .OrderBy(l => l.SortOrder)
            .ToListAsync();

        return View(links);
    }
}
