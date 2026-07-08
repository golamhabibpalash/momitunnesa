using System.Globalization;

namespace School.Web.Helpers;

public static class LocalizationHelper
{
    public static string GetLocalizedContent(string en, string bn)
    {
        var currentCulture = CultureInfo.CurrentUICulture;
        return currentCulture.Name switch
        {
            "bn" or "bn-BD" => bn,
            _ => en
        };
    }

    public static string GetCurrentLanguage()
    {
        var culture = CultureInfo.CurrentUICulture;
        return culture.Name switch
        {
            "bn" or "bn-BD" => "bn",
            _ => "en"
        };
    }

    public static string GetLanguageDirection()
    {
        return GetCurrentLanguage() switch
        {
            "bn" => "ltr",
            _ => "ltr"
        };
    }
}
