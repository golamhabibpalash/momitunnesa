using Microsoft.AspNetCore.Mvc;
using School.Web.Helpers;

namespace School.Web.ViewComponents;

public class LanguageSwitcherViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var currentLanguage = LocalizationHelper.GetCurrentLanguage();
        ViewBag.CurrentLanguage = currentLanguage;
        ViewBag.ReturnUrl = Request.Path.ToString();
        return View();
    }
}
